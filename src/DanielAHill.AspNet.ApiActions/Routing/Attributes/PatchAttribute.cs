// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace DanielAHill.AspNet.ApiActions
{
    public class PatchAttribute : HttpMethodAttribute
    {
        public PatchAttribute() : base("PATCH")
        {
        }
    }
}