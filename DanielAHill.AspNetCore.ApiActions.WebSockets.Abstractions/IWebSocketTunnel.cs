using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketTunnel
    {
        Task SendAsync(Guid commandId, ApiActionResponse response, CancellationToken cancellationToken);
        Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken);

        Task SubscribeAsync(IWebSocketTunnelSubscribable item);
        Task UnsubscribeAsync(Guid id);
        Task UnsubscribeAsync(Guid id, CancellationToken cancellationToken);
    }
}