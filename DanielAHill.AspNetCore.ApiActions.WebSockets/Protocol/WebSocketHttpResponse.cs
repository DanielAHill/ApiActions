using System.Net.WebSockets;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public class WebSocketHttpResponse : IWebSocketHttpResponse
    {
        public WebSocketMessageType Type { get; set; }
        public byte[] Data { get; set; }
    }
}