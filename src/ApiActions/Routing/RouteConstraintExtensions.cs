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
using Microsoft.AspNetCore.Routing;

// ReSharper disable once CheckNamespace - Extension methods should be in the namespace of the type they are extending
namespace Microsoft.AspNet.Routing
{
    internal static class RouteConstraintExtensions
    {
        internal static string GetKey(this IRouteConstraint routeConstraint)
        {
            if (routeConstraint == null) throw new ArgumentNullException(nameof(routeConstraint));

            return routeConstraint is IKeyedRouteContraint keyedRouteConstraint
                ? keyedRouteConstraint.Key
                : string.Concat(routeConstraint.GetType().ToString(), "_", routeConstraint.GetHashCode());
        }
    }
}