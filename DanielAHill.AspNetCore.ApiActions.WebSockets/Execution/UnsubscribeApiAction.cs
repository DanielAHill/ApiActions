using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Execution
{
    public class UnsubscribeApiAction : WebSocketApiAction<WebSocketUnsubscribeRequest>
    {
        public override async Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            await Tunnel.UnsubscribeAsync(Data.Id, cancellationToken);
            return Response();
        }
    }
}
