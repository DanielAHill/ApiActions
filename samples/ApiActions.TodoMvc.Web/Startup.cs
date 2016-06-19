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

using System.Reflection;
using ApiActions.TodoMvc.Domain;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApiActions.TodoMvc.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Required for TodoMvc Back-end automated tests: http://www.todobackend.com/specs/index.html?http://localhost:11324/todo
            services.AddCors();

            // Register single instance of In Memory Todo Repository into Dependency Injection
            services.AddInstance(typeof (ITodoRepository), new RamTodoRepository());

            // Add Web actions for web assembly
            services.AddApiActions(typeof(Startup).GetTypeInfo().Assembly)
                    .AddSwaggerApiActions();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Required for TodoMvc Back-end automated tests: http://www.todobackend.com/specs/index.html?http://localhost:11324/todo
            app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });

            app.UseIISPlatformHandler();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Register Web Action Middleware
            app.UseApiActions();

            // Set json to use Camel Casing and not to explicitly serialize null values
            // Required for TodoMvc Back-end automated tests: http://www.todobackend.com/specs/index.html?http://localhost:11324/todo
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}