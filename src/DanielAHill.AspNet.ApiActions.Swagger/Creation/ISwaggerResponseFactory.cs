using System.Collections.Generic;
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public interface ISwaggerResponseFactory
    {
        IEnumerable<SwaggerResponse> Create(IEnumerable<IApiActionResponseInfo> responseInfos);
    }
}