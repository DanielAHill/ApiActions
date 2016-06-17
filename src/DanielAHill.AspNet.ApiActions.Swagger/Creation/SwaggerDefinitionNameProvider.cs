using System;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerDefinitionNameProvider : ISwaggerDefinitionNameProvider
    {
        public string GetDefinitionName(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.FullName;
        }
    }
}