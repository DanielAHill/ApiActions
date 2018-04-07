using System.Net.Sockets;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IHasWebSocketTunnel
    {
        IWebSocketTunnel Socket { get; }
    }
}
