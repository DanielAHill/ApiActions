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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ApiActions.Versioning
{
    public class RouteRequestVersionParser : IRequestVersionParser
    {
        private readonly string _versionKey;

        public RouteRequestVersionParser(IOptions<ApiActionConfiguration> configurationAccessor)
        {
            _versionKey = configurationAccessor.Value.VersionRouteValueKey;
        }

        public string Parse(HttpContext context, IDictionary<string, object> routeValues)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery (makes code unreadible)
            foreach (var kvp in routeValues)
            {
                if (kvp.Key.Equals(_versionKey, StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                {
                    return kvp.Value.ToString();
                }
            }

            return null;
        }
    }
}