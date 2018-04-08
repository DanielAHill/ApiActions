using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActions.Test.EndToEnd
{
    public abstract class ApiActionsEndToEndTest
    {
        protected static MockApp CreateApp(Action<IServiceCollection> serviceCollectionAction, Action<IApplicationBuilder> applicationBuilderAction)
        {
            return new MockApp(serviceCollectionAction, applicationBuilderAction);
        }

        protected static void Write(HttpResponse response)
        {
            Trace.WriteLine(response.StatusCode);
            foreach (var header in response.Headers)
            {
                Trace.WriteLine(header.ToString());
            }
            Trace.WriteLine(string.Empty);
            
            response.Body.Position = 0;
            Trace.WriteLine(new StreamReader(response.Body).ReadToEnd());
            response.Body.Position = 0;
        }
    }
}
