using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public class TypedSwaggerProperty : SwaggerProperty
    {
        public SwaggerType Type { get; set; }

        protected override void SerializeInner(StringBuilder builder)
        {
            builder.Append("\"type\":\"");
            builder.Append(Type.ToString().ToLowerInvariant());
            builder.Append('"');
        }
    }
}