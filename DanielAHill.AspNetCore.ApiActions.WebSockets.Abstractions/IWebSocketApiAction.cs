namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketApiAction : IApiAction, IWebSocketTunnelSubscribable
    {
        bool IsWebSocket { get; }
    }
}