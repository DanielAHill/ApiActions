namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol.Json
{
    public class JsonWebSocketHttpResponse
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}