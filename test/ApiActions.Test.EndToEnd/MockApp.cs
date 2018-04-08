// Copyright (c) 2018-2018 Daniel A Hill. All rights reserved.
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

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActions.Test.EndToEnd
{
    public class MockApp
    {
        private readonly Action<IServiceCollection> _serviceCollectionAction;
        private readonly Action<IApplicationBuilder> _applicationBuilderAction;

        public MockApp(Action<IServiceCollection> serviceCollectionAction,
            Action<IApplicationBuilder> applicationBuilderAction)
        {
            _serviceCollectionAction = serviceCollectionAction;
            _applicationBuilderAction = applicationBuilderAction;
        }

        public HttpContext Execute(HttpRequestFeature requestFeature)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddRouting();
            _serviceCollectionAction?.Invoke(serviceCollection);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpRequestFeature>(requestFeature);
            featureCollection.Set<IHttpResponseFeature>(new HttpResponseFeature
            {
                Body = new MemoryStream(),
                Headers = new HeaderDictionary()
            });

            var appBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider(), featureCollection);
            _applicationBuilderAction?.Invoke(appBuilder);

            var entryDelegate = appBuilder.Build();

            var context = new DefaultHttpContext(featureCollection)
            {
                RequestServices = appBuilder.ApplicationServices
            };

            entryDelegate(context).Wait();
            context.Response.Body.Position = 0;

            return context;
        }
    }
}