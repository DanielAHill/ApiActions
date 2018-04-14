// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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

using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace ApiActions.AbstractModeling.Application
{
    [TestClass]
    public class JsonAbstractModelApplicatorTest
    {
        [TestMethod]
        public void ApplySingleLayerObject()
        {
            var data = new
            {
                Foo = "bar",
                Multi = new[] {"one", "two"},
                Integer = 42,
                Float = 1.23432,
                Bool = true
            };

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/json");
            mockContext.Setup(c => c.Stream)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));

            var applicator = new JsonAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.AreEqual(0, abstractModel.ValueCount);
            Assert.AreEqual(5, abstractModel.ChildCount);

            var foo = abstractModel["foo"];
            Assert.IsNotNull(foo);
            Assert.AreEqual(0, foo.ChildCount);
            Assert.AreEqual(1, foo.ValueCount);
            Assert.AreEqual("bar", foo.Values[0]);

            var multi = abstractModel["multi"];
            Assert.IsNotNull(multi);
            Assert.AreEqual(0, multi.ChildCount);
            Assert.AreEqual(2, multi.ValueCount);
            Assert.AreEqual("one", multi.Values[0]);
            Assert.AreEqual("two", multi.Values[1]);

            Assert.AreEqual((long) 42, abstractModel["Integer"].Values[0]);
            Assert.AreEqual((decimal) 1.23432, abstractModel["Float"].Values[0]);
            Assert.AreEqual(true, abstractModel["Bool"].Values[0]);
        }

        [TestMethod]
        public void ApplyArrayObject()
        {
            var data = new[] {"one", "two"};

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/json");
            mockContext.Setup(c => c.Stream)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));

            var applicator = new JsonAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.AreEqual(2, abstractModel.ValueCount);
            Assert.AreEqual(0, abstractModel.ChildCount);
            Assert.AreEqual("one", abstractModel.Values[0]);
            Assert.AreEqual("two", abstractModel.Values[1]);
        }

        [TestMethod]
        public void ApplyMultiLayerObject()
        {
            var data = new
            {
                first = "firstvalue",
                inner = new
                {
                    Foo = "bar",
                    Multi = new[] {"one", "two"}
                }
            };

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/json");
            mockContext.Setup(c => c.Stream)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));

            var applicator = new JsonAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            Assert.IsTrue(abstractModel.IsEmpty);

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.IsFalse(abstractModel.IsEmpty);
            Assert.AreEqual(0, abstractModel.ValueCount);
            Assert.AreEqual(2, abstractModel.ChildCount);

            var first = abstractModel["first"];
            Assert.IsNotNull(first);
            Assert.AreEqual(0, first.ChildCount);
            Assert.AreEqual(1, first.ValueCount);
            Assert.AreEqual("firstvalue", first.Values[0]);

            var inner = abstractModel["inner"];
            Assert.IsNotNull(inner);
            Assert.AreEqual(2, inner.ChildCount);
            Assert.AreEqual(0, inner.ValueCount);

            var foo = inner["foo"];
            Assert.IsNotNull(foo);
            Assert.AreEqual(0, foo.ChildCount);
            Assert.AreEqual(1, foo.ValueCount);
            Assert.AreEqual("bar", foo.Values[0]);

            var multi = inner["multi"];
            Assert.IsNotNull(multi);
            Assert.AreEqual(0, multi.ChildCount);
            Assert.AreEqual(2, multi.ValueCount);
            Assert.AreEqual("one", multi.Values[0]);
            Assert.AreEqual("two", multi.Values[1]);
        }

        [TestMethod]
        public void AcceptsApplicationJson()
        {
            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/json");

            var applicator = new JsonAbstractModelApplicator();

            Assert.IsTrue(applicator.ContentTypes.Contains("application/json"));
            Assert.IsTrue(applicator.Handles(mockContext.Object));
        }
    }
}