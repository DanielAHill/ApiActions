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
using System.Reflection;
using DanielAHill.AspNet.ApiActions.Swagger.Creation;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable once CheckNamespace (Extension methods should be in namespace for type they extend)
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerApiActions(this IServiceCollection services)
        {
            return AddSwaggerApiActions(services, null);
        }

        public static IServiceCollection AddSwaggerApiActions(this IServiceCollection services, string routePrefix)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton(typeof(ISwaggerPathFactory), typeof(SwaggerPathFactory));
            services.AddSingleton(typeof(ISwaggerApiActionRegistrationProvider), typeof(SwaggerApiActionRegistrationProvider));
            services.AddSingleton(typeof(ISwaggerDefinitionsFactory), typeof(SwaggerDefinitionsFactory));
            services.AddSingleton(typeof(ISwaggerTypeConverter), typeof(SwaggerTypeConverter));
            
            return services.AddApiActions(typeof(SwaggerServiceCollectionExtensions).GetTypeInfo().Assembly, null, routePrefix);
        }
    }
}