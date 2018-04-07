using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions
{
    public interface IApiActionRouteInitializationContext
    {
        HttpContext HttpContext { get; }
        IReadOnlyDictionary<string, object> RouteDataTokens { get; }
        IDictionary<string, object> RouteValues { get; } 
    }
}