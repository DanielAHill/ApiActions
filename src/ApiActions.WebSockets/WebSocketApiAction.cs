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

using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.AbstractModeling;

namespace ApiActions.WebSockets
{
    public abstract class WebSocketApiAction<TRequest> : ApiAction<TRequest>, IWebSocketApiAction
        where TRequest : class, new()
    {
        private IHasWebSocketTunnel _webSocketTunnelHttpContext;

        public IWebSocketTunnel Socket => _webSocketTunnelHttpContext?.Socket;

        public bool IsWebSocket => _webSocketTunnelHttpContext != null;

        public string CommandId { get; private set; }

        protected override async Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext,
            CancellationToken cancellationToken)
        {
            await base.AppendedInitializeAsync(initializationContext, cancellationToken);

            _webSocketTunnelHttpContext = initializationContext.HttpContext as IHasWebSocketTunnel;
            CommandId = initializationContext.HttpContext.TraceIdentifier;
        }

        #region Connection Events

        public virtual Task OnUnsubscribeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion

        protected Task SendAsync(HttpStatusCode statusCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(statusCode), cancellationToken);
        }

        protected Task SendAsync(int statusCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(statusCode), cancellationToken);
        }

        protected Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Socket.SendAsync(response, cancellationToken);
        }

        protected Task SendAsync<T>(T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(data), cancellationToken);
        }

        protected Task SendAsync<T>(HttpStatusCode statusCode, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(statusCode, data), cancellationToken);
        }

        protected Task SendAsync<T>(int statusCode, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(statusCode, data), cancellationToken);
        }

        protected Task SendAsync(Stream data, string contentType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(data, contentType), cancellationToken);
        }

        protected Task SendAsync(int statusCode, Stream data, string contentType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(Response(statusCode, data, contentType), cancellationToken);
        }

        protected Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Socket.CloseAsync(status, message, cancellationToken);
        }

        protected Task SubscribeAsync()
        {
            return Socket.SubscribeAsync(this);
        }

        protected Task UnsubscribeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Socket.UnsubscribeAsync(CommandId, cancellationToken);
        }
    }

    public abstract class WebSocketApiAction : WebSocketApiAction<AbstractModel>
    {
    }
}