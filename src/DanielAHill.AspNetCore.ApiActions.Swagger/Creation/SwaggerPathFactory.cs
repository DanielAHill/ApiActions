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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DanielAHill.AspNetCore.ApiActions.Introspection;
using DanielAHill.AspNetCore.ApiActions.Swagger.Specification;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

// ReSharper disable once RedundantUsingDirective

namespace DanielAHill.AspNetCore.ApiActions.Swagger.Creation
{
    public class SwaggerPathFactory : ISwaggerPathFactory
    {
        private readonly IApiActionInfoProvider _infoProvider;
        private readonly ISwaggerResponseFactory _responseFactory;
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;
        private readonly ISwaggerSchemaFactory _schemaFactory;
        private readonly string[] _defaultMethods;

        private static readonly Regex RouteParameterRegex = new Regex(@"{(?<name>\w+)(:\w+)?}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public SwaggerPathFactory(IApiActionInfoProvider infoProvider, IOptions<SwaggerOptions> optionsAccessor, 
            ISwaggerResponseFactory responseFactory, 
            ISwaggerDefinitionNameProvider definitionNameProvider,
            ISwaggerSchemaFactory schemaFactory)
        {
            if (optionsAccessor == null) throw new ArgumentNullException(nameof(optionsAccessor));
            _infoProvider = infoProvider ?? throw new ArgumentNullException(nameof(infoProvider));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
            _definitionNameProvider = definitionNameProvider ?? throw new ArgumentNullException(nameof(definitionNameProvider));
            _schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));

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
                Path = "/" + RemoveTypesFromRoute(registration.Route),
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
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (info == null) throw new ArgumentNullException(nameof(info));

            if (registration.Route == null)
            {
                throw new InvalidOperationException("Registration must have a route");
            }
            
            // TODO: Move this method to factory

            if (info.RequestType == null)
            {
                return null;
            }

            var propertyDetails = info.RequestType.GetTypeDetails().PropertyWriters;

            var routeParameterNames = GetRouteParameterNames(registration.Route);

            var routeParameters = new List<SwaggerParameter>();
            var nonRouteParameters = new List<SwaggerParameter>(propertyDetails.Count);

            var includesFile = false;
            
            foreach (var property in propertyDetails)
            {
                var parameter = new SwaggerParameter()
                {
                    Name = property.Name,
                    Schema = _schemaFactory.Create(property.PropertyType, property.PropertyType.GetTypeDetails().PropertyWriters),
                    // TODO: Add Description
                    // TODO: Add Required
                };

                // Check for url
                if (routeParameterNames.Contains(property.Name.ToLowerInvariant()))
                {
                    parameter.In = SwaggerRequestLocation.path;
                    parameter.Required = true; // All Route Parameters are Required
                    routeParameters.Add(parameter);
                    continue;
                }
                
                // Check for file
                if (!includesFile && (typeof (IFormFile).IsAssignableFrom(property.PropertyType) || typeof(Stream).IsAssignableFrom(property.PropertyType)))
                {
                    includesFile = true;
                }

                nonRouteParameters.Add(parameter);
            }

            var supportsBody = !info.Methods.Any(m => "GET".Equals(m, StringComparison.OrdinalIgnoreCase));

            if (!includesFile && supportsBody)
            {
                //if (routeParameterNames.Count == 0)
                {
                    return new []
                    {
                        new SwaggerParameter() { Name="Body", In = SwaggerRequestLocation.body, Required = true, SchemaLink = new SwaggerReferenceLink() { Link = "#/definitions/" + _definitionNameProvider.GetDefinitionName(info.RequestType) } }
                    };
                }

                //throw new NotImplementedException();
            }
            else
            {
                var location = includesFile ? SwaggerRequestLocation.formData : SwaggerRequestLocation.query;
                foreach (var param in nonRouteParameters)
                {
                    param.In = location;
                }
            }

            var parameters = nonRouteParameters.Concat(routeParameters).ToArray();
            return parameters.Length > 0 ? parameters : null;
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

            destination.Item.Parameters = (destination.Item.Parameters ?? new SwaggerParameter[0]).Union(source.Item.Parameters ?? new SwaggerParameter[0]).ToArray();

            foreach (var param in destination.Item.Parameters)
            {
                if (param.In != SwaggerRequestLocation.path)
                {
                    param.Required = false;
                }
            }

            destination.Item.Methods = new SwaggerObjectCollectionFacade<UnofficialPathItemMethod>(duplicateDictionary.Values);
        }

        private static IReadOnlyCollection<string> GetRouteParameterNames(string route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            return RouteParameterRegex.Matches(route).OfType<Match>().Select(m => m.Groups["name"].Value.ToLowerInvariant()).ToList();
        }

        private static string RemoveTypesFromRoute(string route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            return RouteParameterRegex.Replace(route, m => string.Concat("{", m.Groups["name"].Value, "}"));
        }
    }
}