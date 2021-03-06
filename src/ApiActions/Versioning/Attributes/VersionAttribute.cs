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
using ApiActions.Routing;
using ApiActions.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace ApiActions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute, IKeyedRouteContraint, IVersionEdgeFactory
    {
        private readonly ApiActionVersion _minimum;
        private readonly ApiActionVersion _maximum;

        public string Key { get; } = "_Version";

        public VersionAttribute(string minimum)
            : this(minimum, null)
        {
        }

        public VersionAttribute(string minimum, string maximum)
        {
            if (minimum == null) throw new ArgumentNullException(nameof(minimum));
            _minimum = ApiActionVersion.Parse(minimum);
            _maximum = maximum == null ? null : ApiActionVersion.Parse(maximum);
        }

        public ApiActionVersion[] GetVersionEdges()
        {
            return _minimum == _maximum || _maximum == null ? new[] {_minimum} : new[] {_minimum, _maximum};
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var version = httpContext.RequestServices.GetRequiredService<IRequestVersionProvider>()
                .Get(httpContext, values);
            return _minimum <= version && (_maximum == null || version <= _maximum);
        }
    }
}