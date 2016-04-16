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
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;

// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace DanielAHill.AspNet.ApiActions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequireSslAttribute : Attribute, IKeyedRouteContraint
    {
        public string Key { get; } = "_IsHttps";

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string, object> values, RouteDirection routeDirection)
        {
            return httpContext.Request.IsHttps;
        }
    }
}