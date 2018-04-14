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
using ApiActions.WebSockets.Protocol;
using ApiActions.WebSockets.Protocol.Json;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsServiceCollectionExtensions
    {
        public static IServiceCollection AddApiActionsWebSockets(this IServiceCollection services)
        {
            return AddApiActionsWebSockets(services, typeof(WebSocketSession));
        }

        public static IServiceCollection AddApiActionsWebSockets<T>(this IServiceCollection services)
            where T : IWebSocketSession
        {
            return AddApiActionsWebSockets(services, typeof(T));
        }

        private static IServiceCollection AddApiActionsWebSockets(IServiceCollection services,
            Type webSocketSessionType)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(IWebSocketSession), webSocketSessionType);
            services.AddSingleton(typeof(IWebSocketProtocol), typeof(JsonWebSocketProtocol));

            return services;
        }
    }
}