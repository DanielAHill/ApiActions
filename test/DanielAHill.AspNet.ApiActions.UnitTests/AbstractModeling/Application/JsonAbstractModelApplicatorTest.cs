#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Linq;
using System.Threading;
using DanielAHill.AspNet.ApiActions.AbstractModeling;
using DanielAHill.AspNet.ApiActions.AbstractModeling.Application;
using DanielAHill.AspNet.ApiActions.Testing.Helpers;
using Xunit;

namespace DanielAHill.AspNet.ApiActions.UnitTests.AbstractModeling.Application
{
    public class JsonAbstractModelApplicatorTest
    {
        private readonly IAbstractModelApplicator _applicator = new JsonAbstractModelApplicator();

        #region Matching

        [Theory]
        [InlineData("application/json", true)]
        [InlineData("application/JSON", true)]
        [InlineData("APPLICATION/JSON", true)]
        [InlineData("aPplicAtIon/jSon", true)]
        [InlineData("application/jso", false)]
        [InlineData("pplication/json", false)]
        [InlineData("applicationjson", false)]
        [InlineData("application/javascript", false)]
        [InlineData("application/binary", false)]
        [InlineData("application/xml", false)]
        public void MatchesContentType(string contentType, bool isValid)
        {
            var context = new MockModelApplicationRequestContext() {ContentType = contentType};
            Assert.Equal(isValid, _applicator.Handles(context));
        }

        #endregion

        #region Empty
        [Fact]
        public void ParseEmtpy()
        {
            var json = string.Empty;
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(0, model.ChildCount);
            Assert.True(model.IsEmpty, "Should be empty");
        }

        [Fact]
        public void ParseWhitespaceJson()
        {
            var json = "   \t \r\n  \t  ";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(0, model.ChildCount);
            Assert.True(model.IsEmpty, "Should be empty");
        }

        [Fact]
        public void ParseBrackets()
        {
            var json = "{}";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(0, model.ChildCount);
            Assert.True(model.IsEmpty, "Should be empty");
        }

        [Fact]
        public void ParseBracketsWithInnerWhitespace()
        {
            var json = "{   \t \r\n \t  \n}";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(0, model.ChildCount);
            Assert.True(model.IsEmpty, "Should be empty");
        }

        [Fact]
        public void ParseBracketsWithFullWhitespace()
        {
            var json = "   \t   {   \t \r\n \t  \n}   \r\n \t    \r\n   \n\n\n\n\n    ";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(0, model.ChildCount);
            Assert.True(model.IsEmpty, "Should be empty");
        }
        #endregion

        #region SingleLayer
        [Fact]
        public void ParseSimpleProperties()
        {
            // Puposefully support some common non-standard implementations for greater compatability
            var json = @"
{
    ""Quoted Property"" : ""Quoted Value""
            ,
    NonQuotedProperty   :   NonQuotedValue          
                    , 
    TimeStamp : ""2013-12-01T00:00:00-08:00""
}";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(3, model.ChildCount);
            Assert.False(model.IsEmpty, "Cannot be empty");

            AssertModelValueEqual("Quoted Property", "Quoted Value", model);
            AssertModelValueEqual("NonQuotedProperty", "NonQuotedValue", model);
            AssertModelValueEqual("TimeStamp", "2013-12-01T00:00:00-08:00", model);
        }

        [Fact]
        public void ParseDelimitedValues()
        {
            var json = @"
{
    Quote:""\"""",
    ReverseSolidus:""\\"",
    Solidus :""\/"" ,
    Backspace: ""\b"" ,
    FormFeed: ""\f"",
    NewLine:""\n"",
    Return:""\r"",
    Tab:""\t"",
    A:""\a"",
    U:""\u0000"",
    U1:""\u0001""
}";
            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            //Assert.Equal(3, model.ChildCount);
            Assert.False(model.IsEmpty, "Cannot be empty");

            AssertModelValueEqual("Quote", "\"", model);
            AssertModelValueEqual("ReverseSolidus", "\\", model);
            AssertModelValueEqual("Solidus", "/", model);
            AssertModelValueEqual("Backspace", "\b", model);
            AssertModelValueEqual("FormFeed", "\f", model);
            AssertModelValueEqual("NewLine", "\n", model);
            AssertModelValueEqual("Return", "\r", model);
            AssertModelValueEqual("Tab", "\t", model);
            AssertModelValueEqual("A", "a", model);
            AssertModelValueEqual("U", ((char)0).ToString(), model);
            AssertModelValueEqual("U1", ((char)1).ToString(), model);
        }

        [Fact]
        public void ParseValueArray()
        {
            var json = @"
{
    Property:[-1,0,1,2,3,4,5]
}";

            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(1, model.ChildCount);
            Assert.False(model.IsEmpty, "Cannot be empty");
            
            var child = model["Property"];
            Assert.NotNull(child);

            Assert.Equal(7, child.ValueCount);
            Assert.Equal(0, child.ChildCount);

            for (var x = -1; x < 6; x++)
            {
                Assert.Equal(x.ToString(), child.Values.Skip(x + 1).First().ToString());
            }
        }

        #endregion

        [Fact]
        public void ParseNested()
        {
            var json = @"
{
    FirstLevel: {
                    ValueArray: [""0"",""1"",""2"",""3"",""4"",""5""],
                    ObjectArray:[
                                    { Key : 1, Value : ""X1"" },
                                    { Key : 2, Value : ""X2"" },
                                    { Key : 3, Value : ""X3"" }
                                ]
                }
}";

            var model = new AbstractModel();

            _applicator.ApplyAsync(CreateJsonContext(json), model, CancellationToken.None).Wait();

            Assert.Equal(0, model.ValueCount);
            Assert.Equal(1, model.ChildCount);
            Assert.False(model.IsEmpty, "Cannot be empty");

            var firstLevel = model["firstLevel"];
            Assert.NotNull(firstLevel);
            Assert.Equal(0, firstLevel.ValueCount);
            Assert.Equal(2, firstLevel.ChildCount);

            var valueArray = firstLevel["ValueArray"];
            Assert.NotNull(valueArray);
            Assert.Equal(6, valueArray.ValueCount);
            Assert.Equal(0, valueArray.ChildCount);

            for (var x = 0; x < 6; x++)
            {
                Assert.Equal(x.ToString(), valueArray.Values.Skip(x).First().ToString());
            }

            var objectArray = firstLevel["objectArray"];
            Assert.NotNull(objectArray);
            Assert.Equal(3, objectArray.ValueCount);
            Assert.Equal(0, objectArray.ChildCount);

            for (var x = 1; x < 4; x++)
            {
                var item = objectArray.Values.Skip(x - 1).First() as AbstractModel;
                Assert.NotNull(item);
                Assert.Equal(0, item.ValueCount);
                Assert.Equal(2, item.ChildCount);

                Assert.NotNull(item["key"]);
                Assert.Equal(0, item["key"].ChildCount);
                Assert.Equal(1, item["key"].ValueCount);
                Assert.Equal(x.ToString(), item["key"].Values[0]);

                Assert.NotNull(item["value"]);
                Assert.Equal(0, item["value"].ChildCount);
                Assert.Equal(1, item["value"].ValueCount);
                Assert.Equal("X" + x.ToString(), item["value"].Values[0]);
            }
        }

        private static MockModelApplicationRequestContext CreateJsonContext(string json)
        {
            return MockModelApplicationRequestContext.CreateJsonContext(json);
        }


        private static void AssertModelValueEqual(string propertyName, string expectedValue, AbstractModel model)
        {
            Assert.NotNull(model[propertyName]);
            Assert.Equal(1, model[propertyName].ValueCount);
            Assert.Equal(expectedValue, model[propertyName].Values[0]);
            Assert.Equal(expectedValue, model[propertyName].Values[0]);
        }
    }
}
