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
using System.Collections.Generic;

namespace ApiActions.Initialization
{
    internal class ApiActionRegistration : IApiActionRegistration
    {
        public Type ApiActionType { get; }
        public string Route { get; }
        public IReadOnlyDictionary<string, object> Constraints { get; }
        public IReadOnlyDictionary<string, object> Defaults { get; }

        internal ApiActionRegistration(Type type, string route, IReadOnlyDictionary<string, object> constraints,
            IReadOnlyDictionary<string, object> defaults)
        {
            ApiActionType = type;
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
            Defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
        }

        public override string ToString()
        {
            return string.Concat(Route, " [", ApiActionType.Name, "]");
        }
    }
}