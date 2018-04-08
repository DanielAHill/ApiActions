using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IWebSocketSession
    {
        Task ExecuteAsync(HttpContext context);
    }
}