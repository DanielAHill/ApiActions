// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.Serialization;
using Microsoft.AspNetCore.Http;

namespace ApiActions.Responses
{
    public class StringResponse : ApiActionResponse
    {
        private readonly int _statusCode;
        private readonly string _data;

        public StringResponse(HttpStatusCode statusCode, string data)
            : this((int) statusCode, data)
        {
        }

        public StringResponse(int statusCode, string data)
        {
            _statusCode = statusCode;
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public override async Task WriteAsync(HttpContext httpContext, IEdgeSerializer edgeSerializer,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = _statusCode;
            var sWriter = new StreamWriter(httpContext.Response.Body);

            await sWriter.WriteAsync(_data);
            await sWriter.FlushAsync();
        }
    }
}