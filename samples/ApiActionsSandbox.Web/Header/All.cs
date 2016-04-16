using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions;

namespace ApiActionsSandbox.Web.Header
{
    [Description("Returns all HTTP headers provided by client request")]
    [HttpMethod("GET", "POST")]
    [Category("Header Insights")]
    public class All : ApiAction
    {
        [Response(200, typeof(Dictionary<string, string>), "Dictionary of request headers")] 
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
