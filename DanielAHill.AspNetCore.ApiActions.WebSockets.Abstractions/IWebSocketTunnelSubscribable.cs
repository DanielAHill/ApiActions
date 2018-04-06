using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketTunnelSubscribable
    {
        Guid Id { get; }
        Task OnUnsubscribeAsync(CancellationToken cancellationToken);
        Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken);
    }
}