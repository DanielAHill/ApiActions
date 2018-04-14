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

        protected virtual Task SendAsync(ApiActionResponse response)
        {
            return SendAsync(response, default(CancellationToken));
        }

        protected virtual Task SendAsync(ApiActionResponse response, CancellationToken cancellationToken)
        {
            return Socket.SendAsync(response, cancellationToken);
        }
    }

    public abstract class WebSocketApiAction : WebSocketApiAction<AbstractModel>
    {
    }
}