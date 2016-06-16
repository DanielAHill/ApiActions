using System;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NoDocAttribute: Attribute
    {
        public bool HideFromDocumentation { get; private set; }

        public NoDocAttribute()
            :this(true)
        {
            
        }

        public NoDocAttribute(bool hideFromDocumentation)
        {
            HideFromDocumentation = hideFromDocumentation;
        }
    }
}
