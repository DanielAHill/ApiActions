// Copyright (c) 2018-2018 Daniel A Hill. All rights reserved.
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
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;

namespace ApiActions.WebSockets.Protocol
{
    internal class WebSocketTunnelHttpContext : HttpContext
    {
        private readonly HttpContext _socketContext;
        private readonly IWebSocketHttpRequest _request;
        private readonly Func<HttpContext, ApiActionResponse, CancellationToken, Task> _sendDelegate;
        private readonly Func<WebSocketCloseStatus, string, CancellationToken, Task> _closeDelegate;
        private readonly Func<string, CancellationToken, Task> _unsubscribeDelegate;
        private readonly Func<IUnsubscribable, Task> _subscribeDelegate;
        private HttpRequest _httpRequest;
        private WebSocketTunnelHttpResponse _httpResponse;

        public override IFeatureCollection Features => _socketContext.Features;
        public override HttpRequest Request => _httpRequest ?? (_httpRequest = CreateRequest());
        public override HttpResponse Response => _httpResponse ?? (_httpResponse = CreateResponse());
        public override ConnectionInfo Connection => _socketContext.Connection;
        public override WebSocketManager WebSockets => new WebSocketTunnelWebSocketManager();
        public override AuthenticationManager Authentication => _socketContext.Authentication;
        public override ClaimsPrincipal User { get; set; }
        public override IDictionary<object, object> Items { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ISession Session { get; set; }

        public IWebSocketTunnel Socket { get; }

        internal WebSocketTunnelHttpContext(HttpContext socketContext, IWebSocketHttpRequest request,
            Func<HttpContext, ApiActionResponse, CancellationToken, Task> sendDelegate,
            Func<WebSocketCloseStatus, string, CancellationToken, Task> closeDelegate,
            Func<string, CancellationToken, Task> unsubscribeDelegate,
            Func<IUnsubscribable, Task> subscribeDelegate)
        {
            _socketContext = socketContext ?? throw new ArgumentNullException(nameof(socketContext));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _sendDelegate = sendDelegate;
            _closeDelegate = closeDelegate;
            _unsubscribeDelegate = unsubscribeDelegate;
            _subscribeDelegate = subscribeDelegate;

            User = socketContext.User;
            Items = socketContext.Items;
            RequestServices = socketContext.RequestServices;
            RequestAborted = socketContext.RequestAborted;
            TraceIdentifier = _request.CommandId;
            Session = socketContext.Session;

            Socket = new MultiResponseWebSocketTunnel(this, sendDelegate, closeDelegate, unsubscribeDelegate,
                subscribeDelegate);
        }

        public override void Abort()
        {
            _socketContext.Abort();
        }

        private HttpRequest CreateRequest()
        {
            return new WebSocketTunnelHttpRequest(_socketContext.Request, _request, this);
        }

        private WebSocketTunnelHttpResponse CreateResponse()
        {
            return new WebSocketTunnelHttpResponse(this);
        }

        public WebSocketTunnelHttpContext CloneWithoutResponse()
        {
            return new WebSocketTunnelHttpContext(_socketContext, _request, _sendDelegate, _closeDelegate,
                _unsubscribeDelegate, _subscribeDelegate);
        }
    }
}