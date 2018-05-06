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
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ApiActions.WebSockets.Execution
{
    internal class UnsubscribeApiActionRegistration : IApiActionRegistration
    {
        private readonly IOptions<WebSocketApiActionConfiguration> _options;
        public IReadOnlyDictionary<string, object> Constraints { get; } = typeof(UnsubscribeApiAction).GetCustomAttributes(true).Where(a => a is IRouteConstraint).Cast<IRouteConstraint>().ToDictionary(a => a.GetType().Name, a => (object)a);
        public IReadOnlyDictionary<string, object> Defaults => new Dictionary<string, object>();
        public Type ApiActionType => typeof(UnsubscribeApiAction);

        public string Route
        {
            get
            {
                var route = _options.Value.UnsubscribeUrl;

                if (route.StartsWith("/") || route.StartsWith("\\"))
                {
                    route = route.Substring(1);
                }

                return route;
            }
        }

        public UnsubscribeApiActionRegistration(IOptions<WebSocketApiActionConfiguration> options)
        {
            _options = options;
        }
    }
}