using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public class WebSocketHttpRequest : IWebSocketHttpRequest
    {
        public string CommandId { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public IQueryCollection Query { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}