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
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.Serialization;
using Microsoft.AspNetCore.Routing;

namespace DanielAHill.AspNetCore.ApiActions.Execution
{
    internal class ApiActionRouter : IApiActionRouter
    {
        private readonly IApiActionExecutioner _apiActionExecutioner;
        private readonly IEdgeDeserializer _edgeDeserializer;
        private readonly IEdgeSerializerProvider _edgeSerializerProvider;
        private static readonly Task CompletedTask = Task.FromResult(true);

        public ApiActionRouter(IEdgeDeserializer edgeDeserializer,
            IEdgeSerializerProvider edgeSerializerProvider,
            IApiActionExecutioner apiActionExecutioner)
        {
            _edgeDeserializer = edgeDeserializer ?? throw new ArgumentNullException(nameof(edgeDeserializer));
            _edgeSerializerProvider = edgeSerializerProvider ?? throw new ArgumentNullException(nameof(edgeSerializerProvider));
            _apiActionExecutioner = apiActionExecutioner ?? throw new ArgumentNullException(nameof(apiActionExecutioner));
        }

        public Task RouteAsync(RouteContext context)
        {
            context.Handler = new ApiActionRouteHandler(_edgeDeserializer, _edgeSerializerProvider, _apiActionExecutioner, context.RouteData).Handle;
            return CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}