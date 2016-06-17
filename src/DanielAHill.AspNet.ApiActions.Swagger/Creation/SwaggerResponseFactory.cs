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