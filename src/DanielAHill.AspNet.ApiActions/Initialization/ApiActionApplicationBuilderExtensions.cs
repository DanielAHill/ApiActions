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
using System.Reflection;
using DanielAHill.AspNet.ApiActions;
using DanielAHill.AspNet.ApiActions.Execution;
using DanielAHill.AspNet.ApiActions.Routing;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace (Extension methods should be in namespace for type they extend)
namespace Microsoft.AspNet.Builder
{
    public static class ApiActionApplicationBuilderExtensions
    {
        private static bool _alreadyRegistered;

        public static IApplicationBuilder UseApiActions(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (_alreadyRegistered) throw new InvalidOperationException("ApiActions can only be registered once");

            _alreadyRegistered = true;

            var log = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApiAction Initialization");
            var inlineConstraintResolver = app.ApplicationServices.GetRequiredService<IInlineConstraintResolver>();
            var handler = app.ApplicationServices.GetRequiredService<IApiActionRouter>();
            var actionRegistrations = app.ApplicationServices.GetRequiredService<IApiActionRegistrationProvider>().Registrations;
            var globalRouteConstraintFactories = (app.ApplicationServices.GetServices<IGlobalRouteConstraintApplicationFactory>() ?? new IGlobalRouteConstraintApplicationFactory[0]).ToArray();
            var globalRouteDefaultApplicationFactories = (app.ApplicationServices.GetServices<IGlobalRouteDefaultApplicationFactory>() ?? new IGlobalRouteDefaultApplicationFactory[0]).ToArray();

            var routes = new RouteCollection();

            if (actionRegistrations.Count == 0)
            {
                log.LogWarning("No ApiActions Registered. ApiActions will not be active.");
                return app;
            }

            foreach (var registration in actionRegistrations)
            {
                var typeInfo = registration.ApiActionType.GetTypeInfo();

                var dataTokenDictionary = new Dictionary<string, object>
                {
                    {RouteDataKeys.ApiActionType, registration.ApiActionType}
                };

                routes.Add(new TemplateRoute(handler, typeInfo.FullName, registration.Route,
                    GetRouteDefaults(registration, globalRouteDefaultApplicationFactories),
                    GetRouteConstraints(registration, globalRouteConstraintFactories, app.ApplicationServices),
                    dataTokenDictionary,
                    inlineConstraintResolver));
            }

            app.UseRouter(routes);
            log.LogInformation($"Registered {routes.Count} Routes");

            return app;
        }

        private static IDictionary<string, object> GetRouteConstraints(IApiActionRegistration registration,
            IReadOnlyList<IGlobalRouteConstraintApplicationFactory> applicationFactories,
            IServiceProvider serviceProvider)
        {
            var constraints = registration.Constraints as IDictionary<string, object> ?? registration.Constraints.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // ApplyAsync Global Route Constraints
            // ReSharper disable once ForCanBeConvertedToForeach - For loop is faster than foreach
            for (var x = 0; x < applicationFactories.Count; x++)
            {
                var factory = applicationFactories[x];
                if (!factory.Matches(registration.ApiActionType))
                {
                    continue;
                }

                var constraint = factory.Get(registration.ApiActionType, serviceProvider);
                if (constraint == null)
                {
                    continue;
                }

                var key = constraint.GetKey();
                if (!constraints.ContainsKey(key))
                {
                    constraints.Add(key, constraint);
                }
            }

            return constraints;
        }

        private static IDictionary<string, object> GetRouteDefaults(IApiActionRegistration registration, IReadOnlyList<IGlobalRouteDefaultApplicationFactory> applicationFactories)
        {
            var routeDefaults = registration.Defaults as IDictionary<string, object> ?? registration.Defaults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // ApplyAsync Global Route Defaults
            // ReSharper disable once ForCanBeConvertedToForeach - For loop is faster than foreach
            for (var x = 0; x < applicationFactories.Count; x++)
            {
                var factory = applicationFactories[x];
                if (!factory.Matches(registration.ApiActionType))
                {
                    continue;
                }

                foreach (var kvp in factory.Get(registration.ApiActionType).Where(kvp => !routeDefaults.ContainsKey(kvp.Key)))
                {
                    routeDefaults.Add(kvp);
                }
            }

            return routeDefaults;
        }
    }
}