using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions.Execution;
using DanielAHill.AspNetCore.ApiActions.Routing;
using Microsoft.AspNet.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsApplicationBuilderExtensions
    {
        private static bool _requireSsl;
        private static string _socketTunnelUrl;
        private static bool _alreadyRegistered;

        public static IApplicationBuilder UseWebSocketApiActions(this IApplicationBuilder app, string socketTunnelUrl = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (_alreadyRegistered)
            {
                return app;
            }

            _alreadyRegistered = true;

            var configuration = app.ApplicationServices.GetRequiredService<IOptions<WebSocketApiActionConfiguration>>().Value;

            _requireSsl = configuration.RequireSsl;
            _socketTunnelUrl = socketTunnelUrl ?? configuration.SocketTunnelUrl;

            // Ensure API Actions is registered
            app.UseApiActions();

            app.Use(WebSocketApiActionsMiddleware);
            
            return app;
        }

        private static Task WebSocketApiActionsMiddleware(HttpContext context, Func<Task> next)
        {
            if (context.WebSockets.IsWebSocketRequest
                && _socketTunnelUrl.Equals(context.Request.Path, StringComparison.InvariantCultureIgnoreCase)
                && (_requireSsl && context.Request.IsHttps || !_requireSsl))
            {
                return context.RequestServices.GetRequiredService<IWebSocketSession>().ExecuteAsync(context);
            }

            return next();
        }
    }
}