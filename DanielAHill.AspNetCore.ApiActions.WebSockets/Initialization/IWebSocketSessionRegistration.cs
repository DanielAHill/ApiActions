using System;
using System.Collections.Generic;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Initialization
{
    public interface IWebSocketSessionRegistration
    {
        IReadOnlyDictionary<string, object> Constraints { get; }
        IReadOnlyDictionary<string, object> Defaults { get; }
        string Route { get; }
        Type ApiActionType { get; }
    }
}