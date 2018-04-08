using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public interface IWebSocketHttpRequest
    {
        string CommandId { get; }
        string Method { get; }
        string Path { get; }
        IQueryCollection Query { get; }
        IDictionary<string, string[]> Headers { get; }
        string ContentType { get; }
        byte[] Content { get; }
    }
}