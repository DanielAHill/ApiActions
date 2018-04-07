namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketApiAction : IApiAction, IUnsubscribable, IHasWebSocketTunnel
    {
        bool IsWebSocket { get; }
    }
}