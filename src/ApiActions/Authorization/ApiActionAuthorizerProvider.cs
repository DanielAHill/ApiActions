// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiActions.Authorization
{
    public class ApiActionAuthorizerProvider : IAuthFilterProvider
    {
        private readonly IEnumerable<IGlobalAuthFilterApplicationFactory> _globalAuthorizationFilters;
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, IAuthFilter[]> AttributeCache =
            new ConcurrentDictionary<Type, IAuthFilter[]>();

        public ApiActionAuthorizerProvider(IEnumerable<IGlobalAuthFilterApplicationFactory> globalAuthorizationFilters,
            IServiceProvider serviceProvider)
        {
            _globalAuthorizationFilters = globalAuthorizationFilters;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IAuthFilter[] Get(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return AttributeCache.GetOrAdd(type, FindAuthorizationAttributes);
        }

        private IAuthFilter[] FindAuthorizationAttributes(Type type)
        {
            return _globalAuthorizationFilters.Where(f => f.Matches(type)).Select(f => f.Get(type, _serviceProvider))
                .Concat(type.GetTypeInfo().GetCustomAttributes().Where(a => a is IAuthFilter).Cast<IAuthFilter>())
                .ToArray();
        }
    }
}