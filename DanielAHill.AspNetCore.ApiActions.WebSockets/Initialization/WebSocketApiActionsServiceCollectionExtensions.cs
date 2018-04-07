using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsServiceCollectionExtensions
    {
        private static bool _addedCore;

        private static void AddCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (_addedCore)
            {
                return;
            }

            _addedCore = true;
        }

        public static IServiceCollection AddWebSocketSessions(this IServiceCollection services, Assembly assembly, string parentNamespace = null, string routePrefix = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            AddCore(services);

            var preparedNamespace = (parentNamespace ?? assembly.GetName().Name) + ".";
            var preparedParentNamespace = preparedNamespace.Substring(0, preparedNamespace.Length - 1);
            var preparedRoutePrefix = routePrefix == null ? string.Empty : routePrefix + "/";

            var types = assembly.GetTypes()
                .Where(t =>
                {
                    var ti = t.GetTypeInfo();

                    if (ti.Namespace == null || 
                        (!ti.Namespace.StartsWith(preparedNamespace, StringComparison.OrdinalIgnoreCase)) && !ti.Namespace.Equals(preparedParentNamespace))
                    {
                        return false;
                    }

                    return ti.ImplementedInterfaces.Any(i => i == typeof(IWebSocketSession));
                });

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsPublic || typeInfo.IsAbstract)
                {
                    continue;
                }

                services.AddTransient(type);

                var namespaceSection = typeInfo.FullName.Substring(0, typeInfo.FullName.Length - typeInfo.Name.Length - 1);

                namespaceSection = namespaceSection.Length > preparedNamespace.Length ? namespaceSection.Substring(preparedNamespace.Length).Replace('.', '/') : string.Empty;

                var routes = typeInfo.GetCustomAttributes<UrlAttribute>()
                    .Select(ta => ta.GetUrl(namespaceSection))
                    .ToList();

                if (routes.Count == 0)
                {
                    routes.Add(namespaceSection);
                }

                var constraintDictionary = new Dictionary<string, object>();
                foreach (var routeConstraint in typeInfo.GetCustomAttributes().Where(a => a is IRouteConstraint).Cast<IRouteConstraint>())
                {
                    var key = routeConstraint.GetKey();

                    if (constraintDictionary.ContainsKey(key))
                    {
                        throw new InvalidOperationException($"ApiAction {type} has more than one route constraint with the same key: {key}");
                    }

                    constraintDictionary.Add(key, routeConstraint);
                }

                var defaultDictionary = typeInfo.GetCustomAttributes<RouteDefaultAttribute>().ToDictionary(a => a.ParameterName, a => a.Value);

                foreach (var route in routes)
                {
                    services.AddSingleton(typeof(IApiActionRegistration), new WebSocketSessionRegistration(type, preparedRoutePrefix + route, constraintDictionary, defaultDictionary));
                }
            }

            return services;
        }
    }
}
