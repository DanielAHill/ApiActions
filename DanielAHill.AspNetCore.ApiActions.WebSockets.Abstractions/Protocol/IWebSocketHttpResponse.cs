using System.Net.WebSockets;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public interface IWebSocketHttpResponse
    {
        WebSocketMessageType Type { get; }
        byte[] Data { get; }
    }
}