#region Copyright
// Copyright (c) 2016 Daniel A Hill. All rights reserved.
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
#endregion
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.Serialization;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.Responses
{
    public class ObjectResponse : ApiActionResponse
    {
        private readonly int _statusCode;
        private readonly object _data;
           
        public ObjectResponse(HttpStatusCode statusCode)
            : this((int)statusCode, null)
        {
        }

        public ObjectResponse(int statusCode)
            : this(statusCode, null)
        {
        }

        public ObjectResponse(HttpStatusCode statusCode, object data)
            : this((int)statusCode, data)
        {
        }

        public ObjectResponse(object data)
            :this(200, data)
        {
        }

        public ObjectResponse(int statusCode, object data)
        {
            _statusCode = statusCode;
            _data = data;
        }

        public override async Task WriteAsync(HttpContext httpContext, IEdgeSerializer edgeSerializer, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = _statusCode;

            if (_data != null)
            {
                await edgeSerializer.SerializeAsync(_data, httpContext.Response, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}