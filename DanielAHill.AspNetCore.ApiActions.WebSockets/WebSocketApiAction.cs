using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.AbstractModeling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public abstract class WebSocketApiAction<TRequest> : ApiAction<TRequest>, IWebSocketApiAction
        where TRequest : class, new()
    {
        protected IWebSocketTunnel Tunnel { get; private set; }

        public bool IsWebSocket => Tunnel != null;

        public Guid Id { get; } = Guid.NewGuid();

        protected override async Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            await base.AppendedInitializeAsync(initializationContext, cancellationToken);

            var configuration = initializationContext.HttpContext.RequestServices.GetRequiredService<IOptions<WebSocketApiActionConfiguration>>().Value;

            if (initializationContext.HttpContext.Items.TryGetValue(configuration.SocketTunnelItemKey, out var uncastedTunnel))
            {
                Tunnel = (IWebSocketTunnel) uncastedTunnel;
            }
        }

        #region Connection Events

        public virtual Task OnUnsubscribeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Web Socket Methods

        protected virtual Task SendAsync(ApiActionResponse response)
        {
            return SendAsync(response, default(CancellationToken));
        }

        protected virtual Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken)
        {
            return Tunnel.SendAsync(Id, response, cancellationToken);
        }

        protected virtual Task CloseAsync(WebSocketCloseStatus status)
        {
            return CloseAsync(status, null, default(CancellationToken));
        }

        protected virtual Task CloseAsync(WebSocketCloseStatus status, string message)
        {
            return CloseAsync(status, message, default(CancellationToken));
        }

        protected virtual Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return Tunnel.CloseAsync(status, message, cancellationToken);
        }

        #endregion
    }

    public abstract class WebSocketApiAction : WebSocketApiAction<AbstractModel>
    {   
    }
}
