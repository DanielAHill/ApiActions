using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.AbstractModeling;
using DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public abstract class WebSocketApiAction<TRequest> : ApiAction<TRequest>, IWebSocketApiAction
        where TRequest : class, new()
    {
        private WebSocketTunnelHttpContext _webSocketTunnelHttpContext;

        public IWebSocketTunnel Socket => _webSocketTunnelHttpContext?.Socket;

        public bool IsWebSocket => _webSocketTunnelHttpContext != null;

        public string CommandId { get; private set; }

        protected override async Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            await base.AppendedInitializeAsync(initializationContext, cancellationToken);

            _webSocketTunnelHttpContext = initializationContext.HttpContext as WebSocketTunnelHttpContext;
            CommandId = initializationContext.HttpContext.TraceIdentifier;
        }

        #region Connection Events

        public virtual Task OnUnsubscribeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        protected virtual Task SendAsync(ApiActionResponse response)
        {
            return SendAsync(response, default(CancellationToken));
        }

        protected virtual Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken)
        {
            return Socket.SendAsync(response, cancellationToken);
        }
    }

    public abstract class WebSocketApiAction : WebSocketApiAction<AbstractModel>
    {   
    }
}
