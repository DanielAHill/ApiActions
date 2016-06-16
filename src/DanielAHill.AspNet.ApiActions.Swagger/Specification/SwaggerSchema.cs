using System;
using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public class SwaggerSchema : ICustomSwaggerSerializable
    {
        public string Title { get; set; }
        public SwaggerType? Type { get; set; }
        public SwaggerObjectCollectionFacade<SwaggerProperty> Properties { get; set; }

        public void Serialize(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
        {
            builder.Append("{\"title\":\"");
            builder.Append(Title);
            builder.Append("\",");

            if (Type.HasValue)
            {
                builder.Append("\"type\":\"");
                builder.Append(Type.Value.ToString().ToLowerInvariant());
                builder.Append("\",");
            }

            builder.Append("\"properties\":");
            serializeChild(Properties, builder, recursionsLeft);
            builder.Append("}");
        }
    }
}