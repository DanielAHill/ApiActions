using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.Execution;
using DanielAHill.AspNetCore.ApiActions.Serialization;
using DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public class WebSocketSession : IWebSocketSession
    {
        private WebSocket _webSocket;
        private IWebSocketProtocol _protocol;
        private HttpContext _socketContext;
        private readonly IDictionary<string, IUnsubscribable> _subscriptions = new Dictionary<string, IUnsubscribable>();
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

        public virtual async Task ExecuteAsync(HttpContext context)
        {
            await InitializeAsync(context, context.RequestAborted);

            if (!await AuthorizeAsync(context))
            {   // User is not authorized
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }

                return;
            }

            _protocol = context.RequestServices.GetRequiredService<IWebSocketProtocol>();
            _socketContext = context;

            var apiActionMiddlewareExecutioner = context.RequestServices.GetRequiredService<IApiActionMiddlewareExecutioner>();

            try
            {
                _webSocket = await context.WebSockets.AcceptWebSocketAsync();

                // TODO: process in parallel
                while (_webSocket.State == WebSocketState.Open)
                {
                    var request = await ReadNextRequestAsync(context.RequestAborted);

                    // Get request context
                    var requestHttpContext = new WebSocketTunnelHttpContext(_socketContext, request, SendAsync, CloseAsync, UnsubscribeAsync, SubscribeAsync);

                    // Call pipeline with request context
                    await apiActionMiddlewareExecutioner.ExecuteAsync(requestHttpContext);

                    // Write Response
                    await SendAsync(requestHttpContext, requestHttpContext.RequestAborted);
                }
            }
            finally
            {
                if (_webSocket != null && _webSocket.State != WebSocketState.Closed && _webSocket.State != WebSocketState.CloseSent)
                {
                    await CloseAsync(WebSocketCloseStatus.InternalServerError, null, CancellationToken.None);
                }
            }
        }

        protected virtual Task InitializeAsync(HttpContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> AuthorizeAsync(HttpContext context)
        {
            return Task.FromResult(true);
        }

        protected virtual async Task<IWebSocketHttpRequest> ReadNextRequestAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            // TODO: Enforce request maximum length
            using (var memStream = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.CloseStatus.HasValue)
                    {
                        await CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        return null;
                    }

                    memStream.Write(buffer, 0, result.Count);

                } while (!result.EndOfMessage);

                if (!_protocol.SupportsMessageType(result.MessageType))
                {
                    await CloseAsync(WebSocketCloseStatus.InvalidMessageType, $"Protocol {_protocol.GetType().Name} does not support message type {result.MessageType}", CancellationToken.None);
                    return null;
                }

                return _protocol.ParseRequest(_socketContext.Request, result.MessageType, memStream.ToArray());
            }
        }

        protected async Task SendAsync(HttpContext httpContext, ApiActionResponse response, CancellationToken cancellationToken)
        {
            var edgeSerializer = httpContext.RequestServices.GetRequiredService<IEdgeSerializerProvider>().Get(httpContext);
            await response.WriteAsync(httpContext, edgeSerializer, cancellationToken);
            await SendAsync(httpContext, cancellationToken);
        }

        protected async Task SendAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            var response = _protocol.CreateResponse(httpContext.TraceIdentifier, httpContext.Response);

            var hasLock = false;
            try
            {
                await _writeSemaphore.WaitAsync(cancellationToken);
                hasLock = true;
                await _webSocket.SendAsync(new ArraySegment<byte>(response.Data), response.Type, true, cancellationToken);
            }
            finally
            {
                if (hasLock)
                {
                    _writeSemaphore.Release();
                }
            }
        }

        protected virtual async Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            await Task.WhenAll(_webSocket.CloseAsync(status, message, cancellationToken), OnCloseAsync(status, message, cancellationToken));
        }

        protected virtual Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            IEnumerable<Task> onCloseTasks;

            lock (_subscriptions)
            {
                onCloseTasks = _subscriptions.Values.Select(s => s.OnUnsubscribeAsync(cancellationToken));
                _subscriptions.Clear();
            }

            return Task.WhenAll(onCloseTasks);
        }

        protected virtual Task SubscribeAsync(IUnsubscribable item)
        {
            lock (_subscriptions)
            {
                if (_webSocket.State == WebSocketState.Closed)
                {
                    return item.OnUnsubscribeAsync(CancellationToken.None);
                }

                if (_subscriptions.ContainsKey(item.CommandId))
                {
                    throw new InvalidOperationException("Item with id already subscribed");
                }

                _subscriptions.Add(item.CommandId, item);
            }

            return Task.CompletedTask;
        }

        protected virtual Task UnsubscribeAsync(string commandId)
        {
            return UnsubscribeAsync(commandId, CancellationToken.None);
        }

        public virtual async Task UnsubscribeAsync(string commandId, CancellationToken cancellationToken)
        {
            IUnsubscribable subscription;

            lock (_subscriptions)
            {
                if (!_subscriptions.TryGetValue(commandId, out subscription))
                {
                    return;
                }
            }

            await subscription.OnUnsubscribeAsync(cancellationToken);
        }
    }
}