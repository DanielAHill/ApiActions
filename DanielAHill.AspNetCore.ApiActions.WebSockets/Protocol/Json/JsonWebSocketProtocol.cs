using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol.Json
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
                Path = parsed.Path
            };

            if (parsed.Content != null)
            {
                request.Content = Encoding.UTF8.GetBytes(parsed.Content);
            }

            if (parsed.Headers != null && parsed.Headers.Count > 0)
            {
                request.Headers = new Dictionary<string, string[]>(parsed.QueryParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Split(';')));
            }

            if (parsed.QueryParameters != null && parsed.QueryParameters.Count > 0)
            {
                request.Query = new QueryCollection(parsed.QueryParameters.ToDictionary(kvp => kvp.Key, kvp => new StringValues(kvp.Value.Split(';'))));
            }

            return request;
        }

        public IWebSocketHttpResponse CreateResponse(string commandId, HttpResponse response)
        {
            var jsonResponse = new JsonWebSocketHttpResponse
            {
                Id = commandId,
                Code = response.StatusCode,
                ContentType = response.ContentType,
                Content = new StreamReader(response.Body).ReadToEnd()
            };

            return new WebSocketHttpResponse
            {
                Type = WebSocketMessageType.Text,
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonResponse))
            };
        }
    }
}