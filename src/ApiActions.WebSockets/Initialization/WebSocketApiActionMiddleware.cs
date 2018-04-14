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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActions.WebSockets.Initialization
{
    public class WebSocketApiActionMiddleware
    {
        private readonly bool _requireSsl;
        private readonly string _socketTunnelUrl;
        private readonly IServiceProvider _applicationServices;

        public WebSocketApiActionMiddleware(bool requireSsl, string socketTunnelUrl,
            IServiceProvider applicationServices)
        {
            _requireSsl = requireSsl;
            _socketTunnelUrl = socketTunnelUrl ?? throw new ArgumentNullException(nameof(socketTunnelUrl));
            _applicationServices = applicationServices ?? throw new ArgumentNullException(nameof(applicationServices));
        }

        public Task ExecuteAsync(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest
                && _socketTunnelUrl.Equals(context.Request.Path, StringComparison.InvariantCultureIgnoreCase)
                && (_requireSsl && context.Request.IsHttps || !_requireSsl))
            {
                return context.RequestServices.GetRequiredService<IWebSocketSession>()
                    .ExecuteAsync(context, _applicationServices);
            }

            return next();
        }
    }
}