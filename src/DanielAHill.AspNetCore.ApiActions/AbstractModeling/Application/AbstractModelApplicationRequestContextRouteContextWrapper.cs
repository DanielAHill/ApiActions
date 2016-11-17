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

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace DanielAHill.AspNetCore.ApiActions.AbstractModeling.Application
{
    internal class AbstractModelApplicationRequestContextRouteContextWrapper : IAbstractModelApplicationRequestContext
    {
        private readonly RouteContext _routeContext;

        public IServiceProvider RequestServices { get { return _routeContext.HttpContext.RequestServices; } }
        public IDictionary<object, object> Items { get { return _routeContext.HttpContext.Items; } }
        public RouteData RouteData { get { return _routeContext.RouteData; } }
        public ClaimsPrincipal User { get { return _routeContext.HttpContext.User; } }
        public ConnectionInfo Connection { get { return _routeContext.HttpContext.Connection; } }
        public IFeatureCollection Features { get { return _routeContext.HttpContext.Features; } }
        public IRequestCookieCollection Cookies { get { return _routeContext.HttpContext.Request.Cookies; } }
        
        public Stream Stream { get { return _routeContext.HttpContext.Request.Body; }}
        public string ContentType { get { return _routeContext.HttpContext.Request.ContentType; }}

        public string TraceIdentifier { get { return _routeContext.HttpContext.TraceIdentifier; }}

        public IFormCollection Form { get { return _routeContext.HttpContext.Request.Form; }}

        public IQueryCollection Query { get { return _routeContext.HttpContext.Request.Query; }}
        public QueryString QueryString { get { return _routeContext.HttpContext.Request.QueryString; }}

        internal AbstractModelApplicationRequestContextRouteContextWrapper(RouteContext routeContext)
        {
#if DEBUG
            if (routeContext == null) throw new ArgumentNullException(nameof(routeContext));
#endif

            _routeContext = routeContext;
        }
    }
}
