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
using System.Net;
using System.Threading.Tasks;
using ApiActions.AbstractModeling.Application;
using ApiActions.Routing;
using ApiActions.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiActions.Execution
{
    internal class ApiActionRouteHandler
    {
        private readonly IEdgeDeserializer _edgeDeserializer;
        private readonly IEdgeSerializerProvider _edgeSerializerProvider;
        private readonly IApiActionExecutioner _apiActionExecutioner;
        private readonly RouteData _routeData;

        internal ApiActionRouteHandler(IEdgeDeserializer edgeDeserializer,
            IEdgeSerializerProvider edgeSerializerProvider,
            IApiActionExecutioner apiActionExecutioner,
            RouteData routeData)
        {
            _edgeDeserializer = edgeDeserializer ?? throw new ArgumentNullException(nameof(edgeDeserializer));
            _edgeSerializerProvider =
                edgeSerializerProvider ?? throw new ArgumentNullException(nameof(edgeSerializerProvider));
            _apiActionExecutioner =
                apiActionExecutioner ?? throw new ArgumentNullException(nameof(apiActionExecutioner));
            _routeData = routeData;
        }

        internal async Task Handle(HttpContext context)
        {
            var cancellationToken = context.RequestAborted;

            var edgeSerializer = _edgeSerializerProvider.Get(context);
            if (edgeSerializer == null)
            {
                // No Edge Serializer
                context.Response.StatusCode = (int) HttpStatusCode.UnsupportedMediaType;
                return;
            }

            try
            {
                var apiActionType = (Type) _routeData.DataTokens[RouteDataKeys.ApiActionType];

                // Get ApiAction
                var apiAction = (IApiAction) context.RequestServices.GetService(apiActionType);

                if (apiAction == null)
                {
                    try
                    {
                        // TODO: Use refelection to generate compiled anon method to speed up construction
                        apiAction = (IApiAction) Activator.CreateInstance(apiActionType);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Could not instantiate ApiAction {apiActionType}. Ensure action is registered into dependency injection or has a public paramerless constructor.",
                            ex);
                    }
                }

                // DeserializeAsync Element
                var abstractModel = await _edgeDeserializer.DeserializeAsync(
                    new AbstractModelApplicationRequestContextRouteContextWrapper(context, _routeData),
                    cancellationToken);

                // InitializeAsync web action
                await apiAction.InitializeAsync(new ApiActionInitializationContext(context, _routeData, abstractModel),
                    cancellationToken);

                // ExecuteAsync
                var response = await _apiActionExecutioner.ExecuteAsync(apiAction, cancellationToken);

                if (response == null)
                {
                    throw new InvalidOperationException("Web Action Executioner did not return a result");
                }

                // Send Response
                await response.WriteAsync(context, edgeSerializer, cancellationToken);
            }
            catch (Exception ex)
            {
                var options = context.RequestServices.GetRequiredService<IOptions<ApiActionConfiguration>>().Value;

                if (options.ReturnDetailedServerErrors)
                {
                    var response = context.RequestServices.GetRequiredService<IApiActionResponseAbstractFactory>()
                        .Create(ex.GetType())?.Create(ex, ex.GetType());

                    if (response != null)
                    {
                        // Send Detailed Response
                        await response.WriteAsync(context, edgeSerializer, cancellationToken);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}