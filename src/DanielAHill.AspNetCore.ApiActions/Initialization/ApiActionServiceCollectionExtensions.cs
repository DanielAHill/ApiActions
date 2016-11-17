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
using System.Reflection;
using DanielAHill.AspNet.ApiActions;
using DanielAHill.AspNetCore.ApiActions;
using DanielAHill.AspNetCore.ApiActions.AbstractModeling;
using DanielAHill.AspNetCore.ApiActions.AbstractModeling.Application;
using DanielAHill.AspNetCore.ApiActions.Authorization;
using DanielAHill.AspNetCore.ApiActions.Conversion;
using DanielAHill.AspNetCore.ApiActions.Execution;
using DanielAHill.AspNetCore.ApiActions.Initialization;
using DanielAHill.AspNetCore.ApiActions.Introspection;
using DanielAHill.AspNetCore.ApiActions.Responses.Construction;
using DanielAHill.AspNetCore.ApiActions.Routing;
using DanielAHill.AspNetCore.ApiActions.Serialization;
using DanielAHill.AspNetCore.ApiActions.Versioning;
using Microsoft.AspNet.Routing;
using Microsoft.AspNetCore.Routing;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable once CheckNamespace (Extension methods should be in namespace for type they extend)
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiActionsServiceCollectionExtensions
    {
        private static bool _apiActionsCoreAdded;

        #region ApiActions

        private static void AddApiActionsCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (_apiActionsCoreAdded)
            {
                return;
            }

            // Add Routing
            services.AddRouting();

            // Add Introspection. TODO?: Consider separate registration?
            services.AddApiActionIntrospection();
            services.AddSingleton(typeof (IApiActionRegistrationProvider), typeof (ApiActionRegistrationProvider));

            // Authorization
            services.AddSingleton(typeof (IAuthFilterProvider), typeof (ApiActionAuthorizerProvider));

            // Route Execution
            services.AddSingleton(typeof (IApiActionRouter), typeof(ApiActionRouter));
            services.AddSingleton(typeof (IApiActionExecutioner), typeof (ApiActionExecutioner));
            services.AddSingleton(typeof (IRequestModelApiActionExecutioner), typeof (RequestModelApiActionExecutioner));

            // Abstract Model Application
            services.AddSingleton(typeof (IAbstractModelApplicator), typeof (QueryParameterAbstractModelApplicator));
            services.AddSingleton(typeof (IAbstractModelApplicator), typeof (JsonAbstractModelApplicator));
            services.AddSingleton(typeof (IAbstractModelApplicator), typeof (RouteDataAbstractModelApplicator));
            services.AddSingleton(typeof (IAbstractModelApplicator), typeof (FormDataAbstractModelApplicator));

            // Abstract Model Conversion
            services.AddSingleton(typeof (IRequestModelFactory), typeof (RequestModelFactory));

            // Edge Serialization
            services.AddSingleton(typeof (IEdgeDeserializer), typeof (EdgeDeserializer));
            services.AddSingleton(typeof (IEdgeSerializerProvider), typeof (EdgeSerializerProvider));
            services.AddSingleton(typeof (IEdgeSerializer), typeof (JsonEdgeSerializer));

            // Versioning
            services.AddSingleton(typeof (IRequestVersionProvider), typeof (RequestVersionProvider));
            services.AddSingleton(typeof (IRequestVersionParser), typeof (RouteRequestVersionParser));

            // Type Converters
            services.AddSingleton(typeof (IConverterDelegateProvider), typeof (ConverterDelegateProvider));
            services.AddSingleton(typeof (ITypeConverter), typeof (FormFileToStreamConverter));
            services.AddSingleton(typeof (ITypeConverter), typeof (FromStringToApiActionVersionConverter));

            // Response Generation
            services.AddSingleton(typeof (IApiActionResponseAbstractFactory), typeof (ApiActionResponseAbstractFactory));

            // Record Actions are already added to prevent multiple registrations
            _apiActionsCoreAdded = true;
        }

        private static void AddApiActionIntrospection(this IServiceCollection services)
        {
            services.AddSingleton(typeof (IVersionEdgeProvider), typeof (VersionEdgeProvider));
            services.AddSingleton(typeof (IApiActionInfoProvider), typeof (ApiActionInfoProvider));
            
            var defaultIntrospector = new ApiActionIntrospector();
            services.AddSingleton(typeof (IApiActionSummaryFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionDescriptionFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionResponseInfoFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionRequestMethodsFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionRequestTypeFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionCategoryFactory), defaultIntrospector);
            services.AddSingleton(typeof (IApiActionDeprecationFactory), defaultIntrospector);
        }

        public static IServiceCollection AddApiActions(this IServiceCollection services, Assembly assembly, string parentNamespace = null, string routePrefix = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            AddApiActionsCore(services);

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

                    return ti.ImplementedInterfaces.Any(i => i == typeof(IApiAction) || i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestModelApiAction));
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

                if (namespaceSection.Length > preparedNamespace.Length)
                {
                    namespaceSection = namespaceSection.Substring(preparedNamespace.Length).Replace('.', '/');
                }
                else
                {
                    namespaceSection = string.Empty;
                }

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
                    services.AddSingleton(typeof(IApiActionRegistration), new ApiActionRegistration(type, preparedRoutePrefix + route, constraintDictionary, defaultDictionary));
                }
            }

            return services;
        }

        #endregion

        #region Route Constraints

        public static IServiceCollection AddApiRouteConstraint<T>(this IServiceCollection services) where T : IGlobalRouteConstraintApplicationFactory
        {
            return services.AddSingleton(typeof (IGlobalRouteConstraintApplicationFactory), typeof (T));
        }

        public static IServiceCollection AddApiRouteConstraint(this IServiceCollection services, params IGlobalRouteConstraintApplicationFactory[] routeConstraintApplicationFactories)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (routeConstraintApplicationFactories == null) throw new ArgumentNullException(nameof(routeConstraintApplicationFactories));

            foreach (var applicationFactory in routeConstraintApplicationFactories)
            {
                services.AddSingleton(typeof (IGlobalRouteConstraintApplicationFactory), applicationFactory);
            }

            return services;
        }

        public static IServiceCollection AddApiRouteConstraint<T>(this IServiceCollection services, string namespacePrefix)
            where T : IRouteConstraint
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));
            return AddApiRouteConstraint<T>(services, new NamespaceActionTypeFilter(namespacePrefix));
        }

        public static IServiceCollection AddApiRouteConstraint<T>(this IServiceCollection services, params IActionTypeFilter[] filters)
            where T : IRouteConstraint
        {
            return AddApiRouteConstraint(services, new GlobalRouteConstraintApplicationFactoryTypeWrapper(typeof(T), filters ?? new IActionTypeFilter[0]));
        }

        public static IServiceCollection AddApiRouteConstraint(this IServiceCollection services, IRouteConstraint routeConstraint, string namespacePrefix)
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));
            return AddApiRouteConstraint(services, routeConstraint, new NamespaceActionTypeFilter(namespacePrefix));
        }

        public static IServiceCollection AddApiRouteConstraint(this IServiceCollection services,
            IRouteConstraint routeConstraint, params IActionTypeFilter[] filters)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (routeConstraint == null) throw new ArgumentNullException(nameof(routeConstraint));
            return AddApiRouteConstraint(services, new GlobalRouteConstraintApplicationFactoryInstanceWrapper(routeConstraint, filters ?? new IActionTypeFilter[0]));
        }

        #endregion

        #region Route Default Values

        public static IServiceCollection AddApiRouteDefaults(this IServiceCollection services, params IGlobalRouteDefaultApplicationFactory[] routeDefaultApplicatonFactories)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (routeDefaultApplicatonFactories == null) throw new ArgumentNullException(nameof(routeDefaultApplicatonFactories));

            foreach (var applicationFactory in routeDefaultApplicatonFactories)
            {
                services.AddSingleton(typeof(IGlobalRouteDefaultApplicationFactory), applicationFactory);
            }

            return services;
        }

        public static IServiceCollection AddApiRouteDefaults(this IServiceCollection services, IReadOnlyDictionary<string, object> defaults, string namespacePrefix)
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));
            return AddApiRouteDefaults(services, defaults, new NamespaceActionTypeFilter(namespacePrefix));
        }

        public static IServiceCollection AddApiRouteDefaults(this IServiceCollection services,
            IReadOnlyDictionary<string, object> defaults, params IActionTypeFilter[] filters)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (defaults == null) throw new ArgumentNullException(nameof(defaults));

            return AddApiRouteDefaults(services, new GlobalRouteDefaultApplicationFactory(defaults, filters ?? new IActionTypeFilter[0]));
        }

        #endregion

        #region Authentication Filters
        public static IServiceCollection AddApiAuthFilter<T>(this IServiceCollection services) where T:IGlobalAuthFilterApplicationFactory
        {
            return services.AddSingleton(typeof(IGlobalAuthFilterApplicationFactory), typeof(T));
        }

        public static IServiceCollection AddApiAuthFilter(this IServiceCollection services, params IGlobalAuthFilterApplicationFactory[] authFilterApplicationFactories)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (authFilterApplicationFactories == null) throw new ArgumentNullException(nameof(authFilterApplicationFactories));
            
            foreach (var applicationFactory in authFilterApplicationFactories)
            {
                services.AddSingleton(typeof (IGlobalAuthFilterApplicationFactory), applicationFactory);
            }

            return services;
        }

        public static IServiceCollection AddApiAuthFilter<T>(this IServiceCollection services, string namespacePrefix)
            where T : IAuthFilter
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));
            return AddApiAuthFilter<T>(services, new NamespaceActionTypeFilter(namespacePrefix));
        }

        public static IServiceCollection AddApiAuthFilter<T>(this IServiceCollection services, params IActionTypeFilter[] filters)
            where T : IAuthFilter
        {
            return AddApiAuthFilter(services, new GlobalAuthFilterApplicationFactoryTypeWrapper(typeof(T), filters ?? new IActionTypeFilter[0]));
        }

        public static IServiceCollection AddApiAuthFilter(this IServiceCollection services, IAuthFilter authFilter, string namespacePrefix)
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));
            return AddApiAuthFilter(services, authFilter, new NamespaceActionTypeFilter(namespacePrefix));
        }

        public static IServiceCollection AddApiAuthFilter(this IServiceCollection services,
            IAuthFilter authFilter, params IActionTypeFilter[] filters)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (authFilter == null) throw new ArgumentNullException(nameof(authFilter));
            return AddApiAuthFilter(services, new GlobalAuthFilterApplicationFactoryInstanceWrapper(authFilter, filters ?? new IActionTypeFilter[0]));
        }

        #endregion
    }
}