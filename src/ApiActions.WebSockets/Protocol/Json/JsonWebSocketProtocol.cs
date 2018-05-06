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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ApiActions.WebSockets.Protocol.Json
{
    public class JsonWebSocketProtocol : IWebSocketProtocol
    {
        public bool SupportsMessageType(WebSocketMessageType type)
        {
            return type == WebSocketMessageType.Text;
        }

        public IWebSocketHttpRequest ParseRequest(HttpRequest websocketRequest, WebSocketMessageType type, byte[] data)
        {
            var parsed = JsonConvert.DeserializeObject<JsonWebSocketHttpRequest>(Encoding.UTF8.GetString(data));

            var request = new WebSocketHttpRequest
            {
                CommandId = parsed.Id,
                ContentType = parsed.ContentType,
                Method = parsed.Method,
                Path = parsed.Path,
                QueryString = parsed.QueryString
            };

            if (parsed.Content != null)
            {
                request.Content = Encoding.UTF8.GetBytes(parsed.Content);
            }

            if (parsed.Headers != null && parsed.Headers.Count > 0)
            {
                request.Headers =
                    new Dictionary<string, string[]>(
                        parsed.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Split(';')));
            }

            return request;
        }

        public IWebSocketHttpResponse CreateResponse(string commandId, HttpResponse response)
        {
            var content = response.Body.Length == 0 ? null : new StreamReader(response.Body).ReadToEnd();

            var jsonResponse = new JsonWebSocketHttpResponse
            {
                Id = commandId,
                Code = response.StatusCode,
                ContentType = response.ContentType,
                Content = content
            };

            return new WebSocketHttpResponse
            {
                Type = WebSocketMessageType.Text,
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonResponse))
            };
        }
    }
}