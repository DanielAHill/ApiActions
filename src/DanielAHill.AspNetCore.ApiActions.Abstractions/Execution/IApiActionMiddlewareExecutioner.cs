using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.Execution
{
    public interface IApiActionMiddlewareExecutioner
    {
        Task ExecuteAsync(HttpContext context);
        Task ExecuteAsync(HttpContext context, RequestDelegate next);
    }
}