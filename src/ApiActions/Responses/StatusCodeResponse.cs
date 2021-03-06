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

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.Serialization;
using Microsoft.AspNetCore.Http;

namespace ApiActions.Responses
{
    public class StatusCodeResponse : ApiActionResponse
    {
        private readonly int _statusCode;

        public StatusCodeResponse(HttpStatusCode statusCode)
            : this((int) statusCode)
        {
        }

        public StatusCodeResponse(int statusCode)
        {
            _statusCode = statusCode;
        }

        public override Task WriteAsync(HttpContext httpContext, IEdgeSerializer edgeSerializer,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = _statusCode;
            return Task.FromResult(true);
        }
    }
}