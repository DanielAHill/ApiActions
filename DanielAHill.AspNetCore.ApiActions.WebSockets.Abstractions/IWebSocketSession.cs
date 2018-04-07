using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketSession
    {
        IWebSocketTunnel Socket { get; }

        Task InitializeAsync(IWebSocketConnectionInitializationContext initializationContext, CancellationToken cancellationToken);

        Task<bool> AuthorizeAsync(HttpContext context);

        Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken);
    }
}