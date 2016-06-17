using System;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public interface ISwaggerDefinitionNameProvider
    {
        string GetDefinitionName(Type type);
    }
}