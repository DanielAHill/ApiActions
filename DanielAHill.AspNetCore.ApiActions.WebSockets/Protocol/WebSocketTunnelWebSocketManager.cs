using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public class WebSocketTunnelWebSocketManager : WebSocketManager
    {
        public override bool IsWebSocketRequest => false;
        public override IList<string> WebSocketRequestedProtocols => new string[0];

        public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
        {
            throw new InvalidOperationException("Request is not a web socket");
        }
    }
}