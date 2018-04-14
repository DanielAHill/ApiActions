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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebSocketApiActions(this IApplicationBuilder app,
            string socketTunnelUrl = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (app.Properties.ContainsKey("WebSocketApiActionsRegistered"))
            {
                return app;
            }

            app.Properties.Add("WebSocketApiActionsRegistered", true);

            // Ensure API Actions is registered
            app.UseApiActions();

            var configuration = app.ApplicationServices.GetRequiredService<IOptions<WebSocketApiActionConfiguration>>()
                .Value;
            app.Use(new WebSocketApiActionMiddleware(configuration.RequireSsl,
                socketTunnelUrl ?? configuration.SocketTunnelUrl, app.ApplicationServices).ExecuteAsync);

            return app;
        }
    }
}