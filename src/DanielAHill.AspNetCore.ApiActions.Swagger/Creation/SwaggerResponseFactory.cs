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
using System.Linq;
using DanielAHill.AspNetCore.ApiActions.Introspection;
using DanielAHill.AspNetCore.ApiActions.Swagger.Specification;

namespace DanielAHill.AspNetCore.ApiActions.Swagger.Creation
{
    public class SwaggerResponseFactory : ISwaggerResponseFactory
    {
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;

        public SwaggerResponseFactory(ISwaggerDefinitionNameProvider definitionNameProvider)
        {
            _definitionNameProvider = definitionNameProvider ?? throw new ArgumentNullException(nameof(definitionNameProvider));
        }

        public IEnumerable<SwaggerResponse> Create(IEnumerable<IApiActionResponseInfo> responseInfos)
        {
            return responseInfos.Select(Create).ToList();
        }

        private SwaggerResponse Create(IApiActionResponseInfo responseInfo)
        {
            var response = new SwaggerResponse()
            {
                Code = responseInfo.StatusCode.ToString(),
                Description = responseInfo.Description,
            };

            if (responseInfo.ResponseData == null)
            {
                return response;
            }

            var typeDetails = responseInfo.ResponseData.GetTypeDetails();
            var link = "#/definitions/" + _definitionNameProvider.GetDefinitionName(responseInfo.ResponseData);

            if (typeDetails.IsCollection)
            {
                response.Reference = new SwaggerSchemaReferenceLink() { Type = SwaggerType.Array, Link = link };
            }
            else
            {
                response.Reference = new SwaggerReferenceLink { Link = link }; ;
            }

            return response;
        }
    }
}