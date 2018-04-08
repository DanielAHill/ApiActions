using System;
using System.Reflection;
using ApiActions.Versioning;

namespace ApiActions.Conversion
{
    public class FromStringToApiActionVersionConverter : ITypeConverter
    {
        private static readonly TypeInfo StringTypeInfo = typeof(string).GetTypeInfo();
        private static readonly TypeInfo ApiActionVersionTypeInfo = typeof(ApiActionVersion).GetTypeInfo();

        public bool CanConvert(Type sourceType, Type destinationType)
        {
            return destinationType.GetTypeInfo().IsAssignableFrom(ApiActionVersionTypeInfo)
                   && StringTypeInfo.IsAssignableFrom(sourceType.GetTypeInfo());
        }

        public object Convert(object source, Type destinationType)
        {
            return source == null ? null : ApiActionVersion.Parse(source.ToString());
        }
    }
}