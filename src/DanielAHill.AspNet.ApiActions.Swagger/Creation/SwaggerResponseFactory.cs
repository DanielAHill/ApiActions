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

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerResponseFactory : ISwaggerResponseFactory
    {
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;

        public SwaggerResponseFactory(ISwaggerDefinitionNameProvider definitionNameProvider)
        {
            if (definitionNameProvider == null) throw new ArgumentNullException(nameof(definitionNameProvider));
            _definitionNameProvider = definitionNameProvider;
        }

        public IEnumerable<SwaggerResponse> Create(IEnumerable<IApiActionResponseInfo> responseInfos)
        {
            return responseInfos.Select(ri => new SwaggerResponse()
            {
                Code = ri.StatusCode.ToString(),
                Description = ri.Description,
                Reference = ri.ResponseData == null ? null : new SwaggerReferenceLink() { Link = "#/definitions/" + _definitionNameProvider.GetDefinitionName(ri.ResponseData) }
            }).ToList();
        }
    }
}