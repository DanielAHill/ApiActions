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
using System.Net.WebSockets;
using System.Text;
using ApiActions.Test.EndToEnd;
using ApiActions.WebSockets.Initialization;
using ApiActions.WebSockets.Protocol.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace ApiActions.WebSockets
{
    [TestClass]
    public class WebSocketSessionTest : ApiActionsEndToEndTest
    {
        [TestMethod]
        public void ConnectAndClose()
        {
            RunWebSocketScenario(null);
        }

        [TestMethod]
        public void CallSingleApiAction()
        {
            RunWebSocketScenario(ws =>
            {
                var response = SimulateCall(ws, new JsonWebSocketHttpRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Method = "POST",
                    Path = "/echo",
                    Content = JsonConvert.SerializeObject(new {Text = "This is my text"}),
                    ContentType = "application/json"
                });

                Assert.AreEqual(200, response.Code);
            });
        }

        [TestMethod]
        public void BadRequestDataValidationIsPassedBack()
        {
            RunWebSocketScenario(ws =>
            {
                var response = SimulateCall(ws, new JsonWebSocketHttpRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Method = "POST",
                    Path = "/echo"
                });

                Assert.AreEqual(400, response.Code);
            });
        }

        [TestMethod]
        public void InvalidUrl()
        {
            RunWebSocketScenario(ws =>
            {
                var response = SimulateCall(ws, new JsonWebSocketHttpRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Method = "GET",
                    Path = "/this/url/doesnt/exist"
                });

                Assert.AreEqual(404, response.Code);
            });
        }

        private void RunWebSocketScenario(Action<SimulatedWebSocket> performCalls)
        {
            var config = new WebSocketApiActionConfiguration();

            var app = CreateApp(s =>
                {
                    s.AddApiActions(GetType().Assembly);
                    s.AddApiActionsWebSockets();
                },
                a => { a.UseWebSocketApiActions(); });

            var simulatedWebSocket = new SimulatedWebSocket();

            var mockWebSocketFeature = new Mock<IHttpWebSocketFeature>();
            mockWebSocketFeature.Setup(c => c.IsWebSocketRequest).Returns(true);
            mockWebSocketFeature.Setup(c => c.AcceptAsync(It.IsAny<WebSocketAcceptContext>()))
                .ReturnsAsync(simulatedWebSocket);

            var executeTask = app.ExecuteAsync(new HttpRequestFeature
            {
                Path = config.SocketTunnelUrl,
            }, features => { features.Set<IHttpWebSocketFeature>(mockWebSocketFeature.Object); });

            performCalls?.Invoke(simulatedWebSocket);

            simulatedWebSocket.SimulateClose(WebSocketCloseStatus.NormalClosure, "closed");

            executeTask.Wait();
            var resultContext = executeTask.Result;
        }

        private static JsonWebSocketHttpResponse SimulateCall(SimulatedWebSocket webSocket,
            JsonWebSocketHttpRequest request)
        {
            webSocket.SimulateSend(JsonConvert.SerializeObject(request));
            var response = webSocket.SentFromServerQueue.Take();

            Assert.IsNull(webSocket.CloseStatus, "Socket Closed Unexpectedly");

            Assert.IsNotNull(response);

            var parsedResponse =
                JsonConvert.DeserializeObject<JsonWebSocketHttpResponse>(Encoding.UTF8.GetString(response.Data));

            Assert.IsNotNull(parsedResponse);
            Assert.AreEqual(request.Id, parsedResponse.Id);

            return parsedResponse;
        }
    }
}