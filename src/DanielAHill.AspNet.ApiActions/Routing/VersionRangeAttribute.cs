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
using DanielAHill.AspNet.ApiActions.Routing;
using DanielAHill.AspNet.ApiActions.Versioning;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace DanielAHill.AspNet.ApiActions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionRangeAttribute : Attribute, IKeyedRouteContraint, IVersionEdgeFactory
    {
        private readonly ApiActionVersion _minimum;
        private readonly ApiActionVersion _maximum;

        public string Key { get; } = "_Version";

        public VersionRangeAttribute(string minimum)
            :this(minimum, null)
        {
        }

        public VersionRangeAttribute(string minimum, string maximum)
        {
            if (minimum == null) throw new ArgumentNullException(nameof(minimum));
            _minimum = ApiActionVersion.Parse(minimum);
            _maximum = maximum == null ? new ApiActionVersion(int.MaxValue) : ApiActionVersion.Parse(maximum);
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string, object> values, RouteDirection routeDirection)
        {
            var version = httpContext.ApplicationServices.GetRequiredService<IRequestVersionProvider>().Get(httpContext, values);
            return _minimum <= version && version <= _maximum;
        }

        public ApiActionVersion[] GetVersionEdges()
        {
            return _minimum == _maximum ? new [] { _minimum } : new [] { _minimum, _maximum };
        }
    }
}