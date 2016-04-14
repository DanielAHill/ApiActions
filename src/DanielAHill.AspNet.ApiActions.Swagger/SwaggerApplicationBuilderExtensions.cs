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
using DanielAHill.AspNet.ApiActions.Execution;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    public static class SwaggerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            return UseSwagger(app, ((IOptions<SwaggerOptions>)app.ApplicationServices.GetRequiredService(typeof(IOptions<SwaggerOptions>))).Value);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, SwaggerOptions options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var inlineConstraintResolver = app.ApplicationServices.GetRequiredService<IInlineConstraintResolver>();
            var handler = app.ApplicationServices.GetRequiredService<IApiActionRouter>();

            var dataTokenDictionary = new Dictionary<string, object>()
            {
                {RouteDataKeys.ApiActionType, typeof (SwaggerApiAction)},
                {RouteDataKeys.Options, options}
            };
       
            var routeTemplate = options.ApiRoutePrefix + options.RequestUrl;
            var routeName = "OpenApi_" + routeTemplate;

            app.UseRouter(new TemplateRoute(handler, routeName, routeTemplate, null, null, dataTokenDictionary, inlineConstraintResolver));

            return app;
        }
    }
}