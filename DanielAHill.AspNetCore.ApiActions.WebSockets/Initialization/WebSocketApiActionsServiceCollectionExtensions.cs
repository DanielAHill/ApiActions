using System;
using DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol;
using DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Initialization
{
    public static class WebSocketApiActionsServiceCollectionExtensions
    {
        public static IServiceCollection AddApiActionsWebSockets(this IServiceCollection services)
        {
            return AddApiActionsWebSockets(services, typeof(WebSocketSession));
        }

        public static IServiceCollection AddApiActionsWebSockets<T>(this IServiceCollection services) where T:IWebSocketSession
        {
            return AddApiActionsWebSockets(services, typeof(T));
        }

        private static IServiceCollection AddApiActionsWebSockets(IServiceCollection services, Type webSocketSessionType)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(IWebSocketSession), webSocketSessionType);
            services.AddSingleton(typeof(IWebSocketProtocol), typeof(JsonWebSocketProtocol));

            return services;
        }
    }
}