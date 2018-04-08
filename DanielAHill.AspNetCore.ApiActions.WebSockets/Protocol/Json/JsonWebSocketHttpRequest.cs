using System.Collections.Generic;
using System.Net.Mime;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol.Json
{
    public class JsonWebSocketHttpRequest
    {
        public string Id { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}