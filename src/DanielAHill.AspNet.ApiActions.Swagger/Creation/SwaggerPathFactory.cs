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
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerPathFactory : ISwaggerPathFactory
    {
        private readonly IApiActionInfoProvider _infoProvider;
        private readonly string[] _defaultMethods;

        public SwaggerPathFactory(IApiActionInfoProvider infoProvider, IOptions<SwaggerOptions> optionsAccessor)
        {
            if (infoProvider == null) throw new ArgumentNullException(nameof(infoProvider));
            _infoProvider = infoProvider;

            var options = optionsAccessor.Value;
            _defaultMethods = options.DefaultMethods ?? new [] {"GET"};
        }

        public IEnumerable<SwaggerPath> GetPaths(IReadOnlyCollection<IApiActionRegistration> registrations)
        {
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));
            return registrations.Select(GetPath);
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
                            Deprecated = info.IsDeprecated
                            //Responses = GetResponses(info.Responses)
                        }
                    }))
                }
            };

            return path;
        }
    }
}