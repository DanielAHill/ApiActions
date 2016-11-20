﻿#region Copyright
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
using DanielAHill.AspNetCore.ApiActions.AbstractModeling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DanielAHill.AspNetCore.ApiActions.Execution
{
    internal class ApiActionInitializationContext : IApiActionInitializationContext
    {
        public AbstractModel AbstractModel { get; }
        public HttpContext HttpContext { get; }
        public IReadOnlyDictionary<string, object> RouteDataTokens { get; }
        public IDictionary<string, object> RouteValues { get; }

        internal ApiActionInitializationContext(RouteContext routeContext, AbstractModel abstractModel)
        {
#if DEBUG
            if (routeContext == null) throw new ArgumentNullException(nameof(routeContext));
            if (abstractModel == null) throw new ArgumentNullException(nameof(abstractModel));
#endif
            HttpContext = routeContext.HttpContext;
            AbstractModel = abstractModel;
            RouteDataTokens = (IReadOnlyDictionary<string, object>)routeContext.RouteData.DataTokens;
            RouteValues = routeContext.RouteData.Values;
        }
    }
}