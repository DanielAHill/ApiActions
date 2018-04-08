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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiActions.WebSockets.Protocol
{
    public class WebSocketTunnelWebSocketManager : WebSocketManager
    {
        public override bool IsWebSocketRequest => false;
        public override IList<string> WebSocketRequestedProtocols => new string[0];

        public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
        {
            throw new InvalidOperationException("Request is not a web socket");
        }
    }
}