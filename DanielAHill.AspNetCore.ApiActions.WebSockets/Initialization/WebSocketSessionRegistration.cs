using System;
using System.Collections.Generic;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Initialization
{
    internal class WebSocketSessionRegistration : IWebSocketSessionRegistration
    {
        public Type ApiActionType { get; }
        public string Route { get; }
        public IReadOnlyDictionary<string, object> Constraints { get; }
        public IReadOnlyDictionary<string, object> Defaults { get; } 

        internal WebSocketSessionRegistration(Type type, string route, IReadOnlyDictionary<string, object> constraints, IReadOnlyDictionary<string, object> defaults)
        {
#if DEBUG
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (constraints == null) throw new ArgumentNullException(nameof(constraints));
            if (defaults == null) throw new ArgumentNullException(nameof(defaults));
#endif
            ApiActionType = type;
            Route = route;
            Constraints = constraints;
            Defaults = defaults;
        }

        public override string ToString()
        {
            return string.Concat(Route, " [", ApiActionType.Name, "]");
        }
    }
}