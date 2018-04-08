using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public class WebSocketTunnelHttpRequest : HttpRequest
    {
        private readonly HttpRequest _socketRequest;
        private readonly IWebSocketHttpRequest _request;
        public override HttpContext HttpContext { get; }
        public override string Method { get; set; }
        public override string Scheme { get; set; }
        public override bool IsHttps { get; set; }
        public override HostString Host { get; set; }
        public override PathString PathBase { get; set; }
        public override string Protocol { get; set; }
        public override IRequestCookieCollection Cookies { get; set; }
        public override bool HasFormContentType { get; } = false;
        public override IFormCollection Form { get; set; } = new FormCollection(new Dictionary<string, StringValues>());

        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override PathString Path { get; set; }
        public override IHeaderDictionary Headers { get; }

        public WebSocketTunnelHttpRequest(HttpRequest socketRequest, IWebSocketHttpRequest request, HttpContext httpContext)
        {
            _socketRequest = socketRequest ?? throw new ArgumentNullException(nameof(socketRequest));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            IsHttps = socketRequest.IsHttps;
            Host = socketRequest.Host;
            PathBase = socketRequest.PathBase;
            Protocol = "HTTP 1.1";
            Cookies = socketRequest.Cookies;

            Path = request.Path;
            Query = request.Query;
            QueryString = new QueryString(request.Query.Count == 0 ? string.Empty : request.Query.Select(kvp => string.Concat(Uri.EscapeUriString(kvp.Key), "\"", Uri.EscapeUriString(kvp.Value))).Aggregate((a, b) => string.Concat(a, ";", b)));

            Body = new MemoryStream(request.Content);
            ContentLength = Body.Length;
            ContentType = request.ContentType;

            Headers = new HeaderDictionary(request.Headers.ToDictionary(kvp => kvp.Key, kvp => new StringValues(kvp.Value)));
        }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new InvalidOperationException("No Form Data Present");
        }
    }
}