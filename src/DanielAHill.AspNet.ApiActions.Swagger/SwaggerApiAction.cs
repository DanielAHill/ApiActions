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
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Serialization;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using DanielAHill.AspNet.ApiActions.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    //[NoDoc]
    internal class SwaggerApiAction : ApiAction
    {
        private SwaggerOptions _openApiOptions;
        private ApiActionVersion _version;
        private IReadOnlyCollection<IApiActionRegistration> _registrations;
        private IEnumerable<IAbstractModelApplicator> _abstractModelApplicators;
        private IEnumerable<IEdgeSerializer> _edgeSerializers;
        private IApiActionInfoProvider _infoProvider;
        private IVersionEdgeProvider _versionEdgeProvider;
        private string _hostName;

        private static readonly string[] DefaultMethods = {"GET"};

        protected override Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            if (initializationContext == null) throw new ArgumentNullException(nameof(initializationContext));

            var services = initializationContext.HttpContext.RequestServices;

            _openApiOptions = (SwaggerOptions)initializationContext.RouteDataTokens[RouteDataKeys.Options];
            _version = services.GetRequiredService<IRequestVersionProvider>().Get(initializationContext.HttpContext, initializationContext.RouteValues);
            _registrations = services.GetRequiredService<IApiActionRegistrationProvider>().Registrations;
            _infoProvider = services.GetRequiredService<IApiActionInfoProvider>();
            _versionEdgeProvider = services.GetRequiredService<IVersionEdgeProvider>();
            _hostName = initializationContext.HttpContext.Request.Host.Value;
            _abstractModelApplicators = services.GetRequiredService<IEnumerable<IAbstractModelApplicator>>();
            _edgeSerializers = services.GetRequiredService<IEnumerable<IEdgeSerializer>>();
            return Task.FromResult(true);
        }

        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        { 
            var root = new SwaggerSchema()
            {
                Info = new SwaggerInfo()
                {
                    Title = _openApiOptions.Title ?? _hostName + " API",
                    Description = _openApiOptions.Description,
                    TermsOfService = _openApiOptions.TermsOfService,

                    Contact = new SwaggerContact()
                    {
                        Name = _openApiOptions.ContactName,
                        Email = _openApiOptions.ContactEmail,
                        Url = _openApiOptions.ContactUrl
                    },
                    License = new SwaggerLicense()
                    {
                        Name = _openApiOptions.LicenseName,
                        Url = _openApiOptions.LicenseUrl
                    }
                },
                BasePath = _openApiOptions.ApiRoutePrefix,
                Paths = new SwaggerObjectCollectionFacade<SwaggerPath>(_registrations.Select(GetPath)),
                Consumes = _abstractModelApplicators.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray(),
                Produces = _edgeSerializers.SelectMany(a => a.ContentTypes ?? new string[0]).ToArray()
            };

            var versionEdges = _versionEdgeProvider.GetVersionEdges(_registrations.Select(r => r.ApiActionType).ToList());
            if (versionEdges != null && _version != null && versionEdges.Any())
            {
                root.Info.Version = (_version > versionEdges.Max() ? versionEdges.Max() : _version).ToString();
            }
            else
            {
                root.Info.Version = "1.0";
            }


            return Task.FromResult<ApiActionResponse>(new SwaggerApiActionResponse(root));
        }

        private SwaggerPath GetPath(IApiActionRegistration registration)
        {   // TODO: Move registration items to info provider
            var info = _infoProvider.GetInfo(registration.ApiActionType);

            var methods = info.Methods;

            if (methods == null || methods.Length == 0)
            {
                methods = DefaultMethods;
            }

            var path = new SwaggerPath()
            {
                Path = "/" + registration.Route,
                Item = new SwaggerPathItem()
                {
                    Methods = new SwaggerObjectCollectionFacade<UnofficialPathItemMethod>(methods.Select(m => new UnofficialPathItemMethod()
                    {
                        Method = m,
                        Operation = new SwaggerOperation()
                        {
                            Description = info.Description,
                            Tags = info.Categories,
                            Summary = info.Summary
                        }
                    }))
                }
            };

            return path;
        }
    }
}