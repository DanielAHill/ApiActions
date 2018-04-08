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

        public MockApp(Action<IServiceCollection> serviceCollectionAction, Action<IApplicationBuilder> applicationBuilderAction)
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
            featureCollection.Set<IHttpResponseFeature>(new HttpResponseFeature() { Body = new MemoryStream(), Headers = new HeaderDictionary()});

            var appBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider(), featureCollection);
            _applicationBuilderAction?.Invoke(appBuilder);

            var entryDelegate = appBuilder.Build();

            var context = new DefaultHttpContext(featureCollection)
            {
                RequestServices = appBuilder.ApplicationServices
            };

            entryDelegate(context).Wait();

            return context;
        }
    }
}