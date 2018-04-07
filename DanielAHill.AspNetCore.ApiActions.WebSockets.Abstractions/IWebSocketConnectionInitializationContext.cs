namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketConnectionInitializationContext : IApiActionRouteInitializationContext
    {
        IWebSocketTunnel Socket { get; }
    }
}