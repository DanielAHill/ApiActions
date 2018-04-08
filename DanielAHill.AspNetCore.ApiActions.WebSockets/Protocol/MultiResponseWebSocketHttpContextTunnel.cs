using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    internal class MultiResponseWebSocketTunnel : IWebSocketTunnel
    {
        private readonly WebSocketTunnelHttpContext _httpContext;
        private readonly Func<WebSocketCloseStatus, string, CancellationToken, Task> _closeDelegate;
        private readonly Func<string, CancellationToken, Task> _unsubscribeDelegate;
        private readonly Func<IUnsubscribable, Task> _subscribeDelegate;
        private readonly Func<HttpContext, ApiActionResponse, CancellationToken, Task> _sendDelegate;

        internal MultiResponseWebSocketTunnel(WebSocketTunnelHttpContext httpContext, 
            Func<HttpContext, ApiActionResponse, CancellationToken, Task> sendDelegate,
            Func<WebSocketCloseStatus, string, CancellationToken, Task> closeDelegate,
            Func<string, CancellationToken, Task> unsubscribeDelegate,
            Func<IUnsubscribable, Task> subscribeDelegate)
        {
            _httpContext = httpContext;
            _sendDelegate = sendDelegate;
            _closeDelegate = closeDelegate;
            _unsubscribeDelegate = unsubscribeDelegate;
            _subscribeDelegate = subscribeDelegate;
        }

        public Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken)
        {
            // Clone HttpContext without Response
            var newHttpContext = _httpContext.CloneWithoutResponse();

            // Send response using cloned HttpContext
            return _sendDelegate(newHttpContext, response, cancellationToken);
        }

        public Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return _closeDelegate(status, message, cancellationToken);
        }

        public Task SubscribeAsync(IUnsubscribable item)
        {
            return _subscribeDelegate(item);
        }

        public Task UnsubscribeAsync(string commandId)
        {
            return _unsubscribeDelegate(commandId, CancellationToken.None);
        }

        public Task UnsubscribeAsync(string commandId, CancellationToken cancellationToken)
        {
            return _unsubscribeDelegate(commandId, cancellationToken);
        }
    }
}