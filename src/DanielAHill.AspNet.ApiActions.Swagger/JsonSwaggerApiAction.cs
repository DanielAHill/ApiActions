#region Copyright
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
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Introspection.Attributes;
using DanielAHill.AspNet.ApiActions.Swagger.Creation;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using DanielAHill.AspNet.ApiActions.Versioning;
using DanielAHill.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    [Get]
    [NoDoc]
    [UrlSuffix(@"swagger.json")]
    public class JsonSwaggerApiAction : ApiAction<JsonSwaggerApiAction.Request>
    {
        private readonly ISwaggerPathFactory _pathFactory;
        private readonly ISwaggerApiActionRegistrationProvider _registrationProvider;
        private readonly IVersionEdgeProvider _versionEdgeProvider;
        private readonly SwaggerOptions _options;

        private ApiActionVersion _defaultVersion;
        private string _hostName;

        public JsonSwaggerApiAction(ISwaggerPathFactory pathFactory, IOptions<SwaggerOptions> optionsAccessor, ISwaggerApiActionRegistrationProvider registrationProvider, IVersionEdgeProvider versionEdgeProvider)
        {
            _pathFactory = pathFactory;
            _registrationProvider = registrationProvider;
            _versionEdgeProvider = versionEdgeProvider;
            _options = optionsAccessor.Value;
        }

        protected override Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            if (initializationContext == null) throw new ArgumentNullException(nameof(initializationContext));

            var services = initializationContext.HttpContext.RequestServices;
            _defaultVersion = services.GetRequiredService<IRequestVersionProvider>().Get(initializationContext.HttpContext, initializationContext.RouteValues);
            _hostName = initializationContext.HttpContext.Request.Host.Value;
            return Task.FromResult(true);
        }
        
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            var version = Data.Version ?? _defaultVersion;

            var root = new SwaggerBase
            {
                Info = new SwaggerInfo
                {
                    Title = _options.Title ?? _hostName + " API",
                    Description = _options.Description,
                    TermsOfService = _options.TermsOfService,

                    Contact = new SwaggerContact
                    {
                        Name = _options.ContactName,
                        Email = _options.ContactEmail,
                        Url = _options.ContactUrl
                    },
                    License = new SwaggerLicense
                    {
                        Name = _options.LicenseName,
                        Url = _options.LicenseUrl
                    },
                    Version = version.ToString()
                },
                BasePath = _options.ApiRoutePrefix,
                Paths = new SwaggerObjectCollectionFacade<SwaggerPath>(_pathFactory.GetPaths(_registrationProvider.Get(version))),
                //Consumes = _abstractModelApplicators.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray(),
                //Produces = _edgeSerializers.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray()
            };

            return Task.FromResult<ApiActionResponse>(new SwaggerApiActionResponse(root));
        }

        private static SwaggerObjectCollectionFacade<UnofficialResponseStatusCode> GetResponses(IApiActionResponseInfo[] responseInfos)
        {
            if (responseInfos == null || !responseInfos.Any())
            {
                return null;
            }

            return new SwaggerObjectCollectionFacade<UnofficialResponseStatusCode>(
                    responseInfos.Select(ri => new UnofficialResponseStatusCode
                    {
                        StatusCode = ri.StatusCode,
                        Response = new SwaggerResponse
                        {
                            Description = ri.Description,
                            Schema = GetSchema(ri.ResponseData)
                        }
                    }));
        }

        private static SwaggerSchema GetSchema(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var typeDetails = type.GetTypeDetails();

            return new SwaggerSchema()
            {
                Title = type.Name,
                Type = GetSwaggerType(typeDetails),
                Properties = new SwaggerObjectCollectionFacade<SwaggerProperty>(typeDetails.PropertyReaders.Select(GetSwaggerProperty))
            };
        }

        private static SwaggerProperty GetSwaggerProperty(IPropertyReader propertyReader)
        {
#if DEBUG
            if (propertyReader == null) throw new ArgumentNullException(nameof(propertyReader));
#endif
            var typeDetails = propertyReader.PropertyType.GetTypeDetails();
            var swaggerType = GetSwaggerType(typeDetails);

            if (swaggerType.HasValue)
            {
                return new TypedSwaggerProperty()
                {
                    Type = swaggerType.Value,
                    Name = propertyReader.Name
                };
            }

            throw new NotImplementedException();
        }

        private static SwaggerType? GetSwaggerType(ITypeDetails typeDetails)
        {
            if (typeDetails.IsCollection)
            {
                return SwaggerType.Array;
            }

            if (!typeDetails.IsValue)
            {
                return null;
            }

            // TODO: Add Is Type or Nullable of Type helper in Reflection Helper Library
            if (typeDetails.Type == typeof (bool) || typeDetails.Type == typeof (bool?))
            {
                return null;
            }

            // TODO: Add Is One of Types in Reflection Helper Library
            if (typeDetails.Type == typeof (int) 
                || typeDetails.Type == typeof (int?)
                || typeDetails.Type == typeof(uint)
                || typeDetails.Type == typeof(uint?))
            {
                return SwaggerType.Integer;
            }

            if (typeDetails.IsNumeric)
            {
                return SwaggerType.Number;
            }
            
            return SwaggerType.String;
        }

        public class Request
        {
            public ApiActionVersion Version { get; set; }
        }
    }
}