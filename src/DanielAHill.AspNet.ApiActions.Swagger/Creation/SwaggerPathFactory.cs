﻿#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
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
using System.Linq;
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerPathFactory : ISwaggerPathFactory
    {
        private readonly IApiActionInfoProvider _infoProvider;
        private readonly ISwaggerResponseFactory _responseFactory;
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;
        private readonly ISwaggerSchemaFactory _schemaFactory;
        private readonly string[] _defaultMethods;

        public SwaggerPathFactory(IApiActionInfoProvider infoProvider, IOptions<SwaggerOptions> optionsAccessor, 
            ISwaggerResponseFactory responseFactory, 
            ISwaggerDefinitionNameProvider definitionNameProvider,
            ISwaggerSchemaFactory schemaFactory)
        {
            if (infoProvider == null) throw new ArgumentNullException(nameof(infoProvider));
            if (optionsAccessor == null) throw new ArgumentNullException(nameof(optionsAccessor));
            if (responseFactory == null) throw new ArgumentNullException(nameof(responseFactory));
            if (definitionNameProvider == null) throw new ArgumentNullException(nameof(definitionNameProvider));
            if (schemaFactory == null) throw new ArgumentNullException(nameof(schemaFactory));
            _infoProvider = infoProvider;
            _responseFactory = responseFactory;
            _definitionNameProvider = definitionNameProvider;
            _schemaFactory = schemaFactory;

            var options = optionsAccessor.Value;
            _defaultMethods = options.DefaultMethods ?? new [] {"GET"};
        }

        public IEnumerable<SwaggerPath> GetPaths(IReadOnlyCollection<IApiActionRegistration> registrations)
        {
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));
            return CombinePaths(registrations.Select(GetPath));
        }

        protected virtual SwaggerPath GetPath(IApiActionRegistration registration)
        {   // TODO: Move registration items to info provider
            var info = _infoProvider.GetInfo(registration.ApiActionType);

            var methods = info.Methods;

            if (methods == null || methods.Length == 0)
            {
                methods = _defaultMethods;
            }

            var path = new SwaggerPath
            {
                Path = "/" + registration.Route,
                Item = new SwaggerPathItem
                {
                    Methods = new SwaggerObjectCollectionFacade<UnofficialPathItemMethod>(methods.Select(m => new UnofficialPathItemMethod
                    {
                        Method = m,
                        Operation = new SwaggerOperation
                        {
                            Description = info.Description,
                            Tags = info.Categories,
                            Summary = info.Summary,
                            Deprecated = info.IsDeprecated,
                            Responses = new SwaggerObjectCollectionFacade<SwaggerResponse>(_responseFactory.Create(info.Responses))
                        }
                    })),
                    Parameters = GetParameters(registration, info)
                }
            };

            return path;
        }

        private SwaggerParameter[] GetParameters(IApiActionRegistration registration, IApiActionInfo info)
        {
            // TODO: Move this method to factory

            if (info.RequestType == null)
            {
                return null;
            }

            var parameters = new List<SwaggerParameter>();

            var supportsBody = info.Methods.Any(m => !"GET".Equals(m, StringComparison.OrdinalIgnoreCase));

            var propertyDetails = info.RequestType.GetTypeDetails().PropertyWriters;

            foreach (var property in propertyDetails)
            {
                var parameter = new SwaggerParameter()
                {
                    Name = property.Name,
                    Schema = _schemaFactory.Create(property.PropertyType, property.PropertyType.GetTypeDetails().PropertyWriters)
                };

                // Check for url
                
                // Check for file

                // Check for body

                // Set as Query Parameter
                parameter.In = SwaggerRequestLocation.query;

                parameters.Add(parameter);
            }

            return parameters.Count > 0 ? parameters.ToArray() : null;
        }

        private static IEnumerable<SwaggerPath> CombinePaths(IEnumerable<SwaggerPath> paths)
        {
            // Combine duplicate path urls (Swagger 2.0 doesn't support duplicate path urls)
            var combineDictionary = new Dictionary<string, SwaggerPath>();

            foreach (var path in paths)
            {
                SwaggerPath existingPath;
                if (combineDictionary.TryGetValue(path.Path, out existingPath))
                {   // Duplicate Detected
                    Combine(existingPath, path);
                }
                else
                {   // Not a Duplicate
                    combineDictionary.Add(path.Path, path);
                }
            }

            return combineDictionary.Values;
        }

        /// <summary>
        /// Attempts to combine two Swagger Paths, as best as possible.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        /// <remarks>Swagger 2.0 does not support multiple paths with the same url. This may result in incorrect specifications for ApiActions with the same url.</remarks>
        private static void Combine(SwaggerPath destination, SwaggerPath source)
        {
            var duplicateDictionary = destination.Item.Methods.ToDictionary(i => i.Method, i => i);

            foreach (var method in source.Item.Methods)
            {
                if (!duplicateDictionary.ContainsKey(method.Method))
                {
                    duplicateDictionary.Add(method.Method, method);
                }
            }

            destination.Item.Methods = new SwaggerObjectCollectionFacade<UnofficialPathItemMethod>(duplicateDictionary.Values);
        }
    }
}