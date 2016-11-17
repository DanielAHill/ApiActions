using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNetCore.ApiActions;

namespace ApiActions.Sample.Swagger.Api.HelloWorld
{
    [Get]
    [Summary("Says Hello World")]
    [Description("Standard Hello World Implementation")]
    [Category(SwaggerCategories.Simple)]
    public class Get : ApiAction
    {
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Response(new {Hello = "World"});
        }
    }
}