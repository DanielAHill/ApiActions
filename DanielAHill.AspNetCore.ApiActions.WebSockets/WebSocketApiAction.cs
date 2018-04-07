using System;
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
        public IWebSocketTunnel Socket { get; private set; }

        public bool IsWebSocket => Socket != null;

        public Guid Id { get; } = Guid.NewGuid();

        protected override async Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            await base.AppendedInitializeAsync(initializationContext, cancellationToken);

            var configuration = initializationContext.HttpContext.RequestServices.GetRequiredService<IOptions<WebSocketApiActionConfiguration>>().Value;

            if (initializationContext.HttpContext.Items.TryGetValue(configuration.SocketTunnelItemKey, out var uncastedTunnel))
            {
                Socket = (IWebSocketTunnel) uncastedTunnel;
            }
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
            return Socket.SendAsync(Id, response, cancellationToken);
        }
    }

    public abstract class WebSocketApiAction : WebSocketApiAction<AbstractModel>
    {   
    }
}
