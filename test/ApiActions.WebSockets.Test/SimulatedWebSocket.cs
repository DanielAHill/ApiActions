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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiActions.WebSockets
{
    public class SimulatedWebSocket : WebSocket
    {
        private SocketSimulateSendItem _currentReadItem;
        private SocketSimulateSendItem _currentSendItem;
        private readonly Queue<SocketSimulateSendItem> _receiveQueue = new Queue<SocketSimulateSendItem>();
        private readonly SemaphoreSlim _receiveQueueSemaphore = new SemaphoreSlim(0);
        private WebSocketState _state = WebSocketState.Open;
        private WebSocketCloseStatus? _closeStatus;
        private string _closeStatusDescription;

        public override WebSocketCloseStatus? CloseStatus => _closeStatus;
        public override string CloseStatusDescription => _closeStatusDescription;
        public override string SubProtocol { get; }
        public override WebSocketState State => _state;

        public BlockingCollection<SocketSimulateSendItem> SentFromServerQueue { get; } =
            new BlockingCollection<SocketSimulateSendItem>();

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription,
            CancellationToken cancellationToken)
        {
            _state = WebSocketState.Closed;

            var sendItem = new SocketSimulateSendItem
            {
                CloseStatus = closeStatus,
                MessageType = WebSocketMessageType.Close
            };

            if (statusDescription != null)
            {
                sendItem.Data = Encoding.UTF8.GetBytes(statusDescription);
            }

            SentFromServerQueue.Add(sendItem, cancellationToken);

            return Task.CompletedTask;
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription,
            CancellationToken cancellationToken)
        {
            _state = WebSocketState.Closed;

            var sendItem = new SocketSimulateSendItem
            {
                CloseStatus = closeStatus,
                MessageType = WebSocketMessageType.Close
            };

            if (statusDescription != null)
            {
                sendItem.Data = Encoding.UTF8.GetBytes(statusDescription);
            }

            SentFromServerQueue.Add(sendItem, cancellationToken);

            return Task.CompletedTask;
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer,
            CancellationToken cancellationToken)
        {
            if (_currentReadItem == null)
            {
                await _receiveQueueSemaphore.WaitAsync(cancellationToken);
                _currentReadItem = _receiveQueue.Dequeue();
            }

            var copyLength = Math.Min(buffer.Count, _currentReadItem.Data.Length - _currentReadItem.CurrentDataIndex);
            Array.Copy(_currentReadItem.Data, _currentReadItem.CurrentDataIndex, buffer.Array, buffer.Offset,
                copyLength);
            _currentReadItem.CurrentDataIndex += copyLength;

            var messageType = _currentReadItem.MessageType;

            if (messageType == WebSocketMessageType.Close)
            {
                _closeStatus = _currentReadItem.CloseStatus;
                _closeStatusDescription = Encoding.UTF8.GetString(_currentReadItem.Data);
            }

            if (_currentReadItem.Data.Length == _currentReadItem.CurrentDataIndex)
            {
                _currentReadItem = null;
            }

            return new WebSocketReceiveResult(copyLength, messageType, _currentReadItem == null, _closeStatus,
                _closeStatusDescription);
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
            CancellationToken cancellationToken)
        {
            lock (this)
            {
                if (_currentSendItem == null)
                {
                    _currentSendItem = new SocketSimulateSendItem()
                    {
                        Data = new byte[0],
                        MessageType = messageType
                    };
                }

                var newData = new byte[_currentSendItem.Data.Length + buffer.Count];
                Array.Copy(_currentSendItem.Data, 0, newData, 0, _currentSendItem.Data.Length);
                Array.Copy(buffer.Array, buffer.Offset, newData, _currentSendItem.Data.Length, buffer.Count);

                _currentSendItem.Data = newData;

                if (endOfMessage)
                {
                    SentFromServerQueue.Add(_currentSendItem, cancellationToken);
                    _currentSendItem = null;
                }
            }

            return Task.CompletedTask;
        }

        public void SimulateSend(string message)
        {
            SimulateSend(WebSocketMessageType.Text, Encoding.UTF8.GetBytes(message));
        }

        public void SimulateSend(WebSocketMessageType messageType, byte[] data)
        {
            _receiveQueue.Enqueue(new SocketSimulateSendItem {MessageType = messageType, Data = data});
            _receiveQueueSemaphore.Release();
        }

        public void SimulateClose(WebSocketCloseStatus closeStatus, string message)
        {
            _receiveQueue.Enqueue(new SocketSimulateSendItem
            {
                MessageType = WebSocketMessageType.Close,
                Data = Encoding.UTF8.GetBytes(message),
                CloseStatus = closeStatus
            });
            _receiveQueueSemaphore.Release();
        }

        public override void Dispose()
        {
        }
    }

    public class SocketSimulateSendItem
    {
        public int CurrentDataIndex { get; set; }
        public byte[] Data { get; set; }
        public WebSocketMessageType MessageType { get; set; }
        public WebSocketCloseStatus? CloseStatus { get; set; }

        public string GetDataAsString()
        {
            return Encoding.UTF8.GetString(Data);
        }
    }
}