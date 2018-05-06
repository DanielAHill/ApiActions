// Copyright (c) 2018 Daniel A Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.Execution;
using ApiActions.Serialization;
using ApiActions.WebSockets.Protocol;
using ApiActions.WebSockets.Protocol.Tunelling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ApiActions.WebSockets
{
    public class WebSocketSession : IWebSocketSession
    {
        private WebSocket _webSocket;
        private IWebSocketProtocol _protocol;
        private HttpContext _socketContext;

        private readonly IDictionary<string, IUnsubscribable>
            _subscriptions = new Dictionary<string, IUnsubscribable>();

        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

        public virtual async Task ExecuteAsync(HttpContext context, IServiceProvider applicationServices)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (applicationServices == null) throw new ArgumentNullException(nameof(applicationServices));

            await InitializeAsync(context, context.RequestAborted);

            if (!await AuthorizeAsync(context))
            {
                // User is not authorized
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                }
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                }

                return;
            }

            _protocol = context.RequestServices.GetRequiredService<IWebSocketProtocol>();
            _socketContext = context;

            var apiActionMiddlewareExecutioner =
                context.RequestServices.GetRequiredService<IApiActionMiddlewareExecutioner>();

            try
            {
                _webSocket = await context.WebSockets.AcceptWebSocketAsync();

                // TODO: process in parallel
                while (_webSocket.State == WebSocketState.Open)
                {
                    var request = await ReadNextRequestAsync(context.RequestAborted);

                    if (request == null)
                    {
                        continue;
                    }

                    try
                    {
                        using (var serviceScope = context.RequestServices.CreateScope())
                        {
                            var httpContext =
                                CreateTunnelledHttpRequest(context, request, serviceScope.ServiceProvider);

                            // Call pipeline with request context
                            await apiActionMiddlewareExecutioner.ExecuteAsync(httpContext);

                            if (httpContext.Response.Body.CanSeek)
                            {
                                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                            }

                            // Write Response
                            await SendAsync(httpContext, httpContext.RequestAborted);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!await OnErrorAsync(request, ex))
                        {
                            throw;
                        }
                    }
                }
            }
            finally
            {
                if (_webSocket != null && _webSocket.State != WebSocketState.Closed &&
                    _webSocket.State != WebSocketState.CloseSent)
                {
                    await CloseAsync(WebSocketCloseStatus.InternalServerError, null, CancellationToken.None);
                }
            }
        }

        protected virtual Task<bool> OnErrorAsync(IWebSocketHttpRequest request, Exception ex)
        {
            return Task.FromResult(false);
        }

        protected virtual IHttpRequestFeature CreateTunnelHttpRequestFeature(HttpContext webSocketConnectionHttpContext,
            IWebSocketHttpRequest request)
        {
            var requestFeature = new DefaultWebSocketTunnelHttpRequestFeature
            {
                Scheme = webSocketConnectionHttpContext.Request.IsHttps ? "https" : "http",
                Method = request.Method,
                Path = request.Path,
                PathBase = webSocketConnectionHttpContext.Request.PathBase,
                Protocol = "HTTP 1.1",
                RawTarget = "Not supported in web socket tunnel",
                QueryString = request.QueryString,
                Body = new MemoryStream(request.Content ?? new byte[0]),
                Headers = new HeaderDictionary()
            };

            if (request.ContentType != null)
            {
                requestFeature.Headers.Add("Content-Type", new StringValues(request.ContentType));
            }

            if (request.Headers != null)
            {
                foreach (var kvp in request.Headers)
                {
                    requestFeature.Headers.Add(kvp.Key, new StringValues(kvp.Value));
                }
            }

            return requestFeature;
        }

        protected virtual WebSocketTunnelHttpContext CreateTunnelledHttpRequest(HttpContext webSocketConnectionHttpContext, IWebSocketHttpRequest request, IServiceProvider serviceProvider)
        {
            var context = new WebSocketTunnelHttpContext(this);
            context.Features.Set<IHttpRequestFeature>(CreateTunnelHttpRequestFeature(webSocketConnectionHttpContext,
                request));
            context.Features.Set<IHttpRequestLifetimeFeature>(
                new HttpRequestLifetimeFeature {RequestAborted = webSocketConnectionHttpContext.RequestAborted});
            context.Features.Set<IHttpContextAccessor>(new HttpContextAccessor {HttpContext = context});
            context.Features.Set<IHttpConnectionFeature>(webSocketConnectionHttpContext.Features
                .Get<IHttpConnectionFeature>());

            context.User = webSocketConnectionHttpContext.User;
            context.TraceIdentifier = request.CommandId;
            context.RequestServices = serviceProvider;

            return context;
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
                        await CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                            CancellationToken.None);
                        return null;
                    }

                    memStream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (!_protocol.SupportsMessageType(result.MessageType))
                {
                    await CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                        $"Protocol {_protocol.GetType().Name} does not support message type {result.MessageType}",
                        CancellationToken.None);
                    return null;
                }

                return _protocol.ParseRequest(_socketContext.Request, result.MessageType, memStream.ToArray());
            }
        }

        public async Task SendAsync(HttpContext httpContext, ApiActionResponse response,
            CancellationToken cancellationToken)
        {
            var edgeSerializer = httpContext.RequestServices.GetRequiredService<IEdgeSerializerProvider>()
                .Get(httpContext);
            await response.WriteAsync(httpContext, edgeSerializer, cancellationToken);
            httpContext.Response.Body.Position = 0;
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

        public virtual async Task CloseAsync(WebSocketCloseStatus status, string message,
            CancellationToken cancellationToken)
        {
            await Task.WhenAll(_webSocket.CloseAsync(status, message, cancellationToken),
                OnCloseAsync(status, message, cancellationToken));
        }

        protected virtual Task OnCloseAsync(WebSocketCloseStatus status, string message,
            CancellationToken cancellationToken)
        {
            IEnumerable<Task> onCloseTasks;

            lock (_subscriptions)
            {
                onCloseTasks = _subscriptions.Values.Select(s => s.OnUnsubscribeAsync(cancellationToken)).ToList();
                _subscriptions.Clear();
            }

            return Task.WhenAll(onCloseTasks);
        }

        public virtual Task SubscribeAsync(IUnsubscribable item)
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