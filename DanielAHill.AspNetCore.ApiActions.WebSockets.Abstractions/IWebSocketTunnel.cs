using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketTunnel
    {
        Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken);
        Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken);

        Task SubscribeAsync(IUnsubscribable item);
        Task UnsubscribeAsync(string commandId);
        Task UnsubscribeAsync(string commandId, CancellationToken cancellationToken);
    }
}