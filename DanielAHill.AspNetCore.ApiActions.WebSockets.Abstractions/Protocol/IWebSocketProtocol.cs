using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public interface IWebSocketProtocol
    {
        bool SupportsMessageType(WebSocketMessageType type);
        IWebSocketHttpRequest ParseRequest(HttpRequest websocketRequest, WebSocketMessageType type, byte[] data);

        IWebSocketHttpResponse CreateResponse(string commandId, HttpResponse response);
    }
}