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
using ApiActions.Routing;

namespace ApiActions.Initialization
{
    public class GlobalRouteDefaultApplicationFactory : IGlobalRouteDefaultApplicationFactory
    {
        private readonly IReadOnlyDictionary<string, object> _defaults;
        private readonly IActionTypeFilter[] _filters;

        public GlobalRouteDefaultApplicationFactory(IReadOnlyDictionary<string, object> defaults,
            params IActionTypeFilter[] filters)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        public bool Matches(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            if (!typeof(IApiAction).IsAssignableFrom(apiActionType))
                throw new ArgumentException($"{apiActionType} is not an ApiAction", nameof(apiActionType));

            return _filters.Length == 0 || _filters.All(f => f.Matches(apiActionType));
        }

        public IReadOnlyDictionary<string, object> Get(Type apiActionType)
        {
            return _defaults;
        }
    }
}