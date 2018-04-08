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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsApplicationBuilderExtensions
    {
        private static bool _requireSsl;
        private static string _socketTunnelUrl;
        private static bool _alreadyRegistered;

        public static IApplicationBuilder UseWebSocketApiActions(this IApplicationBuilder app,
            string socketTunnelUrl = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (_alreadyRegistered)
            {
                return app;
            }

            _alreadyRegistered = true;

            var configuration = app.ApplicationServices.GetRequiredService<IOptions<WebSocketApiActionConfiguration>>()
                .Value;

            _requireSsl = configuration.RequireSsl;
            _socketTunnelUrl = socketTunnelUrl ?? configuration.SocketTunnelUrl;

            // Ensure API Actions is registered
            app.UseApiActions();

            app.Use(WebSocketApiActionsMiddleware);

            return app;
        }

        private static Task WebSocketApiActionsMiddleware(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest
                && _socketTunnelUrl.Equals(context.Request.Path, StringComparison.InvariantCultureIgnoreCase)
                && (_requireSsl && context.Request.IsHttps || !_requireSsl))
            {
                return context.RequestServices.GetRequiredService<IWebSocketSession>().ExecuteAsync(context);
            }

            return next();
        }
    }
}