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

using DanielAHill.AspNet.ApiActions.Routing;
using Microsoft.AspNet.Routing;

namespace DanielAHill.AspNet.ApiActions.Initialization
{
    public class GlobalRouteConstraintApplicationFactoryInstanceWrapper : GlobalApplicationFactoryInstanceWrapper<IRouteConstraint>, IGlobalRouteConstraintApplicationFactory
    {
        public GlobalRouteConstraintApplicationFactoryInstanceWrapper(IRouteConstraint instance, params IActionTypeFilter[] filters) : base(instance, filters)
        {
        }
    }
}