using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions;

namespace ApiActionsSandbox.Web.Header
{
    [Summary("Single Header Value")]
    [Description("Returns value for specified HTTP header provided by client request")]
    [HttpMethod("GET", "POST")]
    [Category(ApiCategories.Headers)]
    [UrlSuffix("{Header}")]
    public class Single: ApiAction<Single.Request>
    {
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public class Request
        {
            [Required]
            [Description("HTTP Header Name")]
            public string Header { get; set; }
        }
    }
}