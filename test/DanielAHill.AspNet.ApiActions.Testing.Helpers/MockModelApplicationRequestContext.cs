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
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using DanielAHill.AspNet.ApiActions.AbstractModeling.Application;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Routing;

namespace DanielAHill.AspNet.ApiActions.Testing.Helpers
{
    public class MockModelApplicationRequestContext : IAbstractModelApplicationRequestContext
    {
        public IServiceProvider ApplicationServices { get; set; }
        public IServiceProvider RequestServices { get; set; }
        public IDictionary<object, object> Items { get; set; }
        public RouteData RouteData { get; set; }
        public ClaimsPrincipal User { get; set; }
        public ConnectionInfo Connection { get; set; }
        public IFeatureCollection Features { get; set; }
        public IReadableStringCollection Cookies { get; set; }
        public string ContentType { get; set; }
        public IFormCollection Form { get; set; }
        public string TraceIdentifier { get; set; }
        public IReadableStringCollection Query { get; set; }
        public QueryString QueryString { get; set; }
        public Stream Stream { get; set; }

        public static MockModelApplicationRequestContext Create(string contentType, string data)
        {
            var context = new MockModelApplicationRequestContext()
            {
                ContentType = contentType
            };

            if (data != null)
            {
                context.Stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            }

            return context;
        }

        public static MockModelApplicationRequestContext CreateJsonContext(string json)
        {
            return MockModelApplicationRequestContext.Create("application/json", json);
        }
    }
}
