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
using Microsoft.AspNet.Http;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Versioning
{
    public class RouteRequestVersionParser : IRequestVersionParser
    {
        private readonly string _versionRouteValueKey;

        public RouteRequestVersionParser(IOptions<ApiActionConfiguration> configurationAccessor)
        {
            _versionRouteValueKey = configurationAccessor.Value.VersionRouteValueKey;
        }

        public string Parse(HttpContext context, IDictionary<string, object> routeValues)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery (makes code unreadible)
            foreach (var kvp in routeValues)
            {
                if (kvp.Key.Equals(_versionRouteValueKey, StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                {
                    return kvp.Value.ToString();
                }
            }

            return null;
        }
    }
}