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

using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ApiActions.WebSockets.Protocol.Tunelling
{
    public class WebSocketTunnelHttpContext : DefaultHttpContext, IHasWebSocketTunnel, IWebSocketTunnel
    {
        private readonly IWebSocketSession _session;
        private WebSocketTunnelHttpResponse _httpResponse;
        public override HttpResponse Response => _httpResponse ?? (_httpResponse = new WebSocketTunnelHttpResponse(this));
        public IWebSocketTunnel Socket => this;

        public WebSocketTunnelHttpContext(IWebSocketSession session)
        {
            _session = session ?? throw new System.ArgumentNullException(nameof(session));
        }

        Task IWebSocketTunnel.CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return _session.CloseAsync(status, message, cancellationToken);
        }

        Task IWebSocketTunnel.SendAsync(ApiActionResponse response, CancellationToken cancellationToken)
        {
            return _session.SendAsync(CloneWithNoResponse(), response, cancellationToken);
        }

        Task IWebSocketTunnel.SubscribeAsync(IUnsubscribable item)
        {
            return _session.SubscribeAsync(item);
        }

        Task IWebSocketTunnel.UnsubscribeAsync(string commandId)
        {
            return _session.UnsubscribeAsync(commandId);
        }

        Task IWebSocketTunnel.UnsubscribeAsync(string commandId, CancellationToken cancellationToken)
        {
            return _session.UnsubscribeAsync(commandId, cancellationToken);
        }

        public virtual WebSocketTunnelHttpContext CloneWithNoResponse()
        {
            var context = new WebSocketTunnelHttpContext(_session);

            context.Features.Set(Features.Get<IHttpRequestFeature>());
            context.Features.Set(Features.Get<IHttpRequestLifetimeFeature>());
            context.Features.Set(Features.Get<IHttpContextAccessor>());
            context.Features.Set(Features.Get<IHttpConnectionFeature>());

            context.User = User;
            context.TraceIdentifier = TraceIdentifier;
            context.RequestServices = RequestServices;

            return context;
        }
    }
}