using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApiActions.AbstractModeling.Application
{
    [TestClass]
    public class QueryParameterAbstractModelApplicatorTest
    {
        [TestMethod]
        public void ApplyAbstractModel()
        {
            var queryCollection = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"prop1", "prop1stringvalue"},
                {"prop2", "2"}
            });

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(m => m.Query).Returns(queryCollection);

            var abstractModel = new AbstractModel();
            var applicator = new QueryParameterAbstractModelApplicator();

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None);

            Assert.AreEqual(2, abstractModel.ChildCount);

            var prop1 = abstractModel["prop1"];
            Assert.IsNotNull(prop1);
            Assert.AreEqual(0, prop1.ChildCount);
            Assert.AreEqual(1, prop1.ValueCount);
            Assert.AreEqual("prop1stringvalue", prop1.Values[0]);

            var prop2 = abstractModel["prop2"];
            Assert.IsNotNull(prop2);
            Assert.AreEqual(0, prop2.ChildCount);
            Assert.AreEqual(1, prop2.ValueCount);
            Assert.AreEqual("2", prop2.Values[0]);
        }

        [TestMethod]
        public void SupportMultipleValues()
        {
            var queryCollection = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"multi", new [] { "one", "two" }}
            });

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(m => m.Query).Returns(queryCollection);

            var abstractModel = new AbstractModel();
            var applicator = new QueryParameterAbstractModelApplicator();

            applicator.ApplyAsync(mockContext.Object, abstractModel, CancellationToken.None);

            Assert.AreEqual(1, abstractModel.ChildCount);

            var prop1 = abstractModel["multi"];
            Assert.IsNotNull(prop1);
            Assert.AreEqual(0, prop1.ChildCount);
            Assert.AreEqual(2, prop1.ValueCount);
            Assert.AreEqual("one", prop1.Values[0]);
            Assert.AreEqual("two", prop1.Values[1]);
        }

        [TestMethod]
        public void TriggersForAllRequests()
        {
            Assert.IsTrue(new QueryParameterAbstractModelApplicator().Handles(new Mock<IAbstractModelApplicationRequestContext>().Object));
        }

        [TestMethod]
        public void DoesNotSpecifyContentTypes()
        {
            Assert.IsNull(new QueryParameterAbstractModelApplicator().ContentTypes);
        }
    }
}
