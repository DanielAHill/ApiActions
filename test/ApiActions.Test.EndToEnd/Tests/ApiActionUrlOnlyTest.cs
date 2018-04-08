using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiActions.Test.EndToEnd.Tests
{
    [TestClass]
    public class ApiActionUrlOnlyTest : ApiActionsEndToEndTest
    {
        [TestMethod]
        public void NoFilterApiAction()
        {
            var app = CreateApp(
                s => { s.AddApiActions(this.GetType().Assembly); },
                a => { a.UseApiActions(); });

            var context = app.Execute(new HttpRequestFeature() { Method = "GET", Path = "/ApiActions/UrlOnly/NoFilter"});           

            Write(context.Response);
            
            Assert.AreEqual(200, context.Response.StatusCode);
        }
    }
}
