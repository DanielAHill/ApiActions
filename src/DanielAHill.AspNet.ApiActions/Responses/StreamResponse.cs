#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
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
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions.Serialization;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.Responses
{
    public class StreamResponse : ApiActionResponse, IDisposable
    {
        private readonly int _statusCode;
        private readonly string _contentType;
        private readonly Stream _stream;

        public StreamResponse(Stream stream, string contentType)
            :this(200, stream, contentType)
        {
        }

        public StreamResponse(HttpStatusCode statusCode, Stream stream, string contentType)
            :this((int)statusCode, stream, contentType)
        {
        }

        public StreamResponse(int statusCode, Stream stream, string contentType)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            _statusCode = statusCode;
            _contentType = contentType;
            _stream = stream;
        }

        public override Task WriteAsync(HttpContext httpContext, IEdgeSerializer edgeSerializer, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = _statusCode;
            httpContext.Response.ContentType = _contentType;
            return _stream.CopyToAsync(httpContext.Response.Body, 4096, cancellationToken);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}