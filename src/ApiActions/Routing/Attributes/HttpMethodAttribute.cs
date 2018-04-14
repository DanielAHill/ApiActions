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
using ApiActions.Introspection;
using ApiActions.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace ApiActions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpMethodAttribute : Attribute, IKeyedRouteContraint, IHasRequestMethods
    {
        private readonly string[] _methods;

        public string Key { get; } = "_HttpMethod";

        public IReadOnlyCollection<string> RequestMethods => _methods;

        // ReSharper disable once MemberCanBeProtected.Global
        public HttpMethodAttribute(params string[] methods)
        {
            if (methods == null) throw new ArgumentNullException(nameof(methods));

            _methods = new string[methods.Length];

            for (var x = 0; x < _methods.Length; x++)
            {
                _methods[x] = methods[x].ToLowerInvariant();
            }
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var requestMethod = httpContext.Request.Method.ToLowerInvariant();
            var match = false;

            for (var x = 0; !match && x < _methods.Length; x++)
            {
                match = _methods[x] == requestMethod;
            }

            return match;
        }
    }
}