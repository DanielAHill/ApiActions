using System.Threading;
using System.Threading.Tasks;

namespace ApiActions.Test.EndToEnd.ApiActions.UrlOnly.NoFilter
{
    public class NoFilterApiAction : ApiAction
    {
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Response(new { ApiAction = GetType().Name });
        }
    }
}
