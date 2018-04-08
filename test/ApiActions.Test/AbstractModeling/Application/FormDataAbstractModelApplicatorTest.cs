// Copyright (c) 2018-2018 Daniel A Hill. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApiActions.AbstractModeling.Application
{
    [TestClass]
    public class FormDataAbstractModelApplicatorTest
    {
        [TestMethod]
        public void DoesNotTriggerForNullContentType()
        {
            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            Assert.IsFalse(new FormDataAbstractModelApplicator().Handles(mockContext.Object));
        }

        [TestMethod]
        public void ApplyWwwFormUrlEncoded()
        {
            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                {"foo", "bar"},
                {"multi", new[] {"one", "two"}}
            });

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/x-www-form-urlencoded");
            mockContext.Setup(c => c.Form).Returns(formCollection);

            var applicator = new FormDataAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            Assert.IsTrue(applicator.Handles(mockContext.Object));
            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.AreEqual(0, abstractModel.ValueCount);
            Assert.AreEqual(2, abstractModel.ChildCount);

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
        }

        [TestMethod]
        public void ApplyMultiPartFormData()
        {
            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                {"foo", "bar"},
                {"multi", new[] {"one", "two"}}
            });

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("multipart/form-data");
            mockContext.Setup(c => c.Form).Returns(formCollection);

            var applicator = new FormDataAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            Assert.IsTrue(applicator.Handles(mockContext.Object));
            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.AreEqual(0, abstractModel.ValueCount);
            Assert.AreEqual(2, abstractModel.ChildCount);

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
        }

        [TestMethod]
        public void ApplyFileUploads()
        {
            var fileFormCollection = new FormFileCollection
            {
                new FormFile(new MemoryStream(), 0, 25, null, "filename.txt")
                {
                    Headers = new HeaderDictionary(new Dictionary<string, StringValues>
                    {
                        {"Content-Disposition", "form-data; filename=\"filename.txt\"; name=\"name\""}
                    })
                }
            };

            var formCollection = new FormCollection(new Dictionary<string, StringValues>(), fileFormCollection);

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.Form).Returns(formCollection);

            var applicator = new FormDataAbstractModelApplicator();
            var abstractModel = new AbstractModel();

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None).Wait();

            Assert.AreEqual(0, abstractModel.ValueCount);
            Assert.AreEqual(1, abstractModel.ChildCount);

            var name = abstractModel["name"];
            Assert.IsNotNull(name);
            Assert.AreEqual(0, name.ChildCount);
            Assert.AreEqual(1, name.ValueCount);
            Assert.IsNotNull(name.Values[0]);
            Assert.IsInstanceOfType(name.Values[0], typeof(IFormFile));
            Assert.AreEqual("filename.txt", ((IFormFile) name.Values[0]).FileName);
        }

        [TestMethod]
        public void AcceptsApplicationWwwFormUrlEncoded()
        {
            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("application/x-www-form-urlencoded");

            var applicator = new FormDataAbstractModelApplicator();

            Assert.IsTrue(applicator.ContentTypes.Contains("application/x-www-form-urlencoded"));
            Assert.IsTrue(applicator.Handles(mockContext.Object));
        }

        [TestMethod]
        public void AcceptsMultiPartFormData()
        {
            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(c => c.ContentType).Returns("multipart/form-data");

            var applicator = new FormDataAbstractModelApplicator();

            Assert.IsTrue(applicator.ContentTypes.Contains("multipart/form-data"));
            Assert.IsTrue(applicator.Handles(mockContext.Object));
        }
    }
}