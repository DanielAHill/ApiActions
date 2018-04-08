using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.ObjectPool;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets.Protocol
{
    public class WebSocketTunnelHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext { get; }
        public override int StatusCode { get; set; }
        public override IHeaderDictionary Headers { get; }
        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override IResponseCookies Cookies { get; }
        public override bool HasStarted => false;

        public WebSocketTunnelHttpResponse(HttpContext context)
        {
            HttpContext = context ?? throw new ArgumentNullException(nameof(context));
            Headers = new HeaderDictionary();
            Body = new MemoryStream();
            Cookies = new ResponseCookies(Headers, new DefaultObjectPool<StringBuilder>(new DefaultPooledObjectPolicy<StringBuilder>()));
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }
}