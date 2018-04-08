namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public class WebSocketApiActionConfiguration
    {
        public bool RequireSsl { get; set; }
        public string SocketTunnelUrl { get; set; } = "/ws";
        public string UnsubscribeUrlSuffix { get; set; } = "/unsubscribe/{id:Guid}";
    }
}