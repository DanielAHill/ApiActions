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

using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActionsSandbox.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Web actions for web assembly
            services.AddApiActions(typeof (Startup).GetTypeInfo().Assembly)
                    .AddSwaggerApiActions();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Register Web Action Middleware
            app.UseApiActions();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
