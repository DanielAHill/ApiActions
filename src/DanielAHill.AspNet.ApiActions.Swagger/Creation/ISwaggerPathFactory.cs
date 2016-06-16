using System.Collections.Generic;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public interface ISwaggerPathFactory
    {
        IEnumerable<SwaggerPath> GetPaths(IReadOnlyCollection<IApiActionRegistration> registrations);
    }
}