using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public static class HasWebSocketTunnelExtensions
    {
        public static Task CloseAsync(this IHasWebSocketTunnel socket, WebSocketCloseStatus status)
        {
            return socket.CloseAsync(status, null, default(CancellationToken));
        }

        public static Task CloseAsync(this IHasWebSocketTunnel socket, WebSocketCloseStatus status, string message)
        {
            return socket.CloseAsync(status, message, default(CancellationToken));
        }

        public static Task CloseAsync(this IHasWebSocketTunnel socket, WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return socket.Socket.CloseAsync(status, message, cancellationToken);
        }

    }
}