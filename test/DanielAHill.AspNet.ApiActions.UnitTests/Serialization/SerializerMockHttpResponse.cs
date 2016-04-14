﻿#region Copyright
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
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.UnitTests.Serialization
{
    internal class SerializerMockHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext { get; }
        public override int StatusCode { get; set; }
        public override IHeaderDictionary Headers { get; }
        public override Stream Body { get; set; } = new MemoryStream();
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override IResponseCookies Cookies { get; }
        public override bool HasStarted { get; }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotSupportedException();
        }

        internal string GetBodyAsString()
        {
            Body.Seek(0, SeekOrigin.Begin);
            return new StreamReader(Body).ReadToEnd();
        }
    }
}