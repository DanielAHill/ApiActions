using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Execution
{
    public class WebSocketTunnel : IWebSocketTunnel
    {
        private readonly WebSocket _webSocket;
        private readonly IDictionary<Guid, IWebSocketTunnelSubscribable> _subscriptions;

        public WebSocketTunnel(WebSocket webSocket)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            _subscriptions = new Dictionary<Guid, IWebSocketTunnelSubscribable>();
        }

        public Task SubscribeAsync(IWebSocketTunnelSubscribable item)
        {
            lock (_subscriptions)
            {
                if (_webSocket.State == WebSocketState.Closed)
                {
                    return item.OnCloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                }

                if (_subscriptions.ContainsKey(item.Id))
                {
                    throw new InvalidOperationException("Item with id already subscribed");
                }

                _subscriptions.Add(item.Id, item);
            }

            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(Guid id)
        {
            return UnsubscribeAsync(id, default(CancellationToken));
        }

        public async Task UnsubscribeAsync(Guid id, CancellationToken cancellationToken)
        {
            IWebSocketTunnelSubscribable subscription;

            lock (_subscriptions)
            {
                if (!_subscriptions.TryGetValue(id, out subscription))
                {
                    return;
                }
            }

            await subscription.OnUnsubscribeAsync(cancellationToken);
        }

        public Task SendAsync(Guid commandId, ApiActionResponse response, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return _webSocket.CloseAsync(status, message, cancellationToken);
        }

        private Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            IEnumerable<Task> onCloseTasks;

            lock (_subscriptions)
            {
                onCloseTasks = _subscriptions.Values.Select(s => s.OnCloseAsync(status, message, cancellationToken));
            }

            return Task.WhenAll(onCloseTasks);
        }
    }
}