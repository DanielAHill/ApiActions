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
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace ApiActions.AbstractModeling.Application
{
    internal class AbstractModelApplicationRequestContextRouteContextWrapper : IAbstractModelApplicationRequestContext
    {
        private readonly HttpContext _httpContext;

        public IServiceProvider RequestServices => _httpContext.RequestServices;
        public IDictionary<object, object> Items => _httpContext.Items;
        public RouteData RouteData { get; }

        public ClaimsPrincipal User => _httpContext.User;
        public ConnectionInfo Connection => _httpContext.Connection;
        public IFeatureCollection Features => _httpContext.Features;
        public IRequestCookieCollection Cookies => _httpContext.Request.Cookies;

        public Stream Stream => _httpContext.Request.Body;
        public string ContentType => _httpContext.Request.ContentType;

        public string TraceIdentifier => _httpContext.TraceIdentifier;

        public IFormCollection Form => _httpContext.Request.Form;

        public IQueryCollection Query => _httpContext.Request.Query;
        public QueryString QueryString => _httpContext.Request.QueryString;

        internal AbstractModelApplicationRequestContextRouteContextWrapper(HttpContext httpContext, RouteData routeData)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            RouteData = routeData ?? throw new ArgumentNullException(nameof(routeData));
        }
    }
}