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
using DanielAHill.AspNet.ApiActions.AbstractModeling.Application;
using DanielAHill.AspNet.ApiActions.Serialization;
using DanielAHill.AspNet.ApiActions.Swagger.Creation;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using DanielAHill.AspNet.ApiActions.Versioning;
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
        private readonly IEnumerable<IAbstractModelApplicator> _abstractModelApplicators;
        private readonly IEnumerable<IEdgeSerializer> _edgeSerializers;
        private readonly ISwaggerDefinitionsFactory _swaggerDefinitionsFactory;
        private readonly SwaggerOptions _options;

        private ApiActionVersion _defaultVersion;
        private string _hostName;

        public JsonSwaggerApiAction(ISwaggerPathFactory pathFactory, 
            IOptions<SwaggerOptions> optionsAccessor, 
            ISwaggerApiActionRegistrationProvider registrationProvider, 
            IEnumerable<IAbstractModelApplicator> abstractModelApplicators,
            IEnumerable<IEdgeSerializer> edgeSerializers,
            ISwaggerDefinitionsFactory swaggerDefinitionsFactory)
        {
            _pathFactory = pathFactory;
            _registrationProvider = registrationProvider;
            _abstractModelApplicators = abstractModelApplicators;
            _edgeSerializers = edgeSerializers;
            _swaggerDefinitionsFactory = swaggerDefinitionsFactory;
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

            var versionMatchedApiActions = _registrationProvider.Get(version);

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

                Paths = new SwaggerObjectCollectionFacade<SwaggerPath>(_pathFactory.GetPaths(versionMatchedApiActions)),

                Consumes = _abstractModelApplicators.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray(),
                Produces = _edgeSerializers.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray(),
                Definitions =  new SwaggerObjectCollectionFacade<SwaggerDefinition>(_swaggerDefinitionsFactory.Create(versionMatchedApiActions))
            };

            if (_options.ExternalDocumentationDescription != null || _options.ExternalDocumentationUrl != null)
            {
                root.ExternalDocs = new SwaggerExternalDocumentation()
                {
                    Description = _options.ExternalDocumentationDescription,
                    Url = _options.ExternalDocumentationUrl
                };
            }

            return Task.FromResult<ApiActionResponse>(new SwaggerApiActionResponse(root));
        }

        public class Request
        {
            public ApiActionVersion Version { get; set; }
        }
    }
}