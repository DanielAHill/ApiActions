using System.Threading;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApiActions.AbstractModeling.Application
{
    [TestClass]
    public class RouteDataAbstractModelApplicatorTest
    {
        [TestMethod]
        public void ApplyAbstractModel()
        {
            var routeData = new RouteData();
            routeData.Values.Add("prop1", "prop1stringvalue");
            routeData.Values.Add("prop2", 2);

            var mockContext = new Mock<IAbstractModelApplicationRequestContext>();
            mockContext.Setup(m => m.RouteData).Returns(routeData);

            var abstractModel = new AbstractModel();
            var applicator = new RouteDataAbstractModelApplicator();

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
            Assert.AreEqual(2, prop2.Values[0]);
        }

        [TestMethod]
        public void TriggersForAllRequests()
        {
            Assert.IsTrue(new RouteDataAbstractModelApplicator().Handles(new Mock<IAbstractModelApplicationRequestContext>().Object));
        }

        [TestMethod]
        public void DoesNotSpecifyContentTypes()
        {
            Assert.IsNull(new RouteDataAbstractModelApplicator().ContentTypes);
        }
    }
}
