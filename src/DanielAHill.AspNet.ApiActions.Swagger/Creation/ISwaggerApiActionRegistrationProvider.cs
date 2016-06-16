using System;
using System.Collections.Generic;
using DanielAHill.AspNet.ApiActions.Versioning;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public interface ISwaggerApiActionRegistrationProvider
    {
        IReadOnlyCollection<IApiActionRegistration> Get(ApiActionVersion version);
    }

    public class SwaggerApiActionRegistrationProvider : ISwaggerApiActionRegistrationProvider
    {
        private readonly IApiActionRegistrationProvider _registrationProvider;

        public SwaggerApiActionRegistrationProvider(IApiActionRegistrationProvider registrationProvider)
        {
            if (registrationProvider == null) throw new ArgumentNullException(nameof(registrationProvider));
            _registrationProvider = registrationProvider;
        }

        public IReadOnlyCollection<IApiActionRegistration> Get(ApiActionVersion version)
        {
            throw new NotImplementedException();
        }
    }
}