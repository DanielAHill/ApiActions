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
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.Serialization;
using Microsoft.AspNetCore.Routing;

namespace DanielAHill.AspNetCore.ApiActions.Execution
{
    internal class ApiActionRouter : IApiActionRouter
    {
        private readonly IApiActionExecutioner _apiActionExecutioner;
        private readonly IRequestModelApiActionExecutioner _requestModelApiActionExecutioner;
        private readonly IEdgeDeserializer _edgeDeserializer;
        private readonly IEdgeSerializerProvider _edgeSerializerProvider;
        private static readonly Task CompletedTask = Task.FromResult(true);

        public ApiActionRouter(IEdgeDeserializer edgeDeserializer,
            IEdgeSerializerProvider edgeSerializerProvider,
            IApiActionExecutioner apiActionExecutioner, 
            IRequestModelApiActionExecutioner requestModelApiActionExecutioner)
        {
            if (edgeDeserializer == null) throw new ArgumentNullException(nameof(edgeDeserializer));
            if (edgeSerializerProvider == null) throw new ArgumentNullException(nameof(edgeSerializerProvider));
            if (apiActionExecutioner == null) throw new ArgumentNullException(nameof(apiActionExecutioner));
            if (requestModelApiActionExecutioner == null) throw new ArgumentNullException(nameof(requestModelApiActionExecutioner));
            _edgeDeserializer = edgeDeserializer;
            _edgeSerializerProvider = edgeSerializerProvider;
            _apiActionExecutioner = apiActionExecutioner;
            _requestModelApiActionExecutioner = requestModelApiActionExecutioner;
        }

        public Task RouteAsync(RouteContext context)
        {
            context.Handler = new ApiActionRouteHandler(_edgeDeserializer, _edgeSerializerProvider, _apiActionExecutioner, _requestModelApiActionExecutioner, context.RouteData).Handle;
            return CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}