﻿// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiActions.WebSockets
{
    public interface IWebSocketSession
    {
        Task ExecuteAsync(HttpContext context, IServiceProvider applicationServices);
        Task CloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken = default(CancellationToken));
        Task SubscribeAsync(IUnsubscribable item);
        Task UnsubscribeAsync(string commandId, CancellationToken cancellationToken = default(CancellationToken));
        Task SendAsync(HttpContext httpContext, ApiActionResponse response, CancellationToken cancellationToken = default(CancellationToken));
    }
}