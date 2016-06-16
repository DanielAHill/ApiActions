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
                            //Responses = GetResponses(info.Responses)
                        }
                    }))
                }
            };

            return path;
        }
    }
}