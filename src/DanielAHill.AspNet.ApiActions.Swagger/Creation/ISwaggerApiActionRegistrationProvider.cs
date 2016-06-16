using System.Collections.Generic;
using DanielAHill.AspNet.ApiActions.Versioning;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public interface ISwaggerApiActionRegistrationProvider
    {
        IReadOnlyCollection<IApiActionRegistration> Get(ApiActionVersion version);
    }
}