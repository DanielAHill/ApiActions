using System;

namespace DanielAHill.AspNet.ApiActions.Introspection
{
    public interface IApiActionDeprecationFactory
    {
        bool CreateIsDeprecated(Type apiActionType);
    }
}