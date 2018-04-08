using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DanielAHill.AspNetCore.ApiActions.Execution
{
    internal class ApiActionMiddlewareExecutioner : IApiActionMiddlewareExecutioner
    {
        public Task ExecuteAsync(HttpContext context)
        {
            return ExecuteAsync(context, NotFoundRequestDelegateAsync );
        }

        public Task ExecuteAsync(HttpContext context, RequestDelegate next)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return new RouterMiddleware(next, context.RequestServices.GetRequiredService<ILoggerFactory>(), ApiActionApplicationBuilderExtensions.RouteCollection).Invoke(context);
        }

        private static Task NotFoundRequestDelegateAsync(HttpContext context)
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }
    }
}