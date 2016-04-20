using System;
using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public abstract class SwaggerProperty : ICustomSwaggerSerializable
    {
        public string Name { get; set; }

        public void Serialize(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
        {
            builder.Append('"');
            builder.Append(Name);
            builder.Append("\":{");
            SerializeInner(builder);
            builder.Append('}');
        }

        protected abstract void SerializeInner(StringBuilder builder);
    }
}