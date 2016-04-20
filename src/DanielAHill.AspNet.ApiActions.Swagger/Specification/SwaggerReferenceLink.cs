using System;
using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public class SwaggerReferenceLink : ICustomSwaggerSerializable
    {
        public string Link { get; set; }

        public void Serialize(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
        {
            builder.Append("{\"$ref\":\"");
            builder.Append(Link);
            builder.Append("\"}");
        }
    }
}
