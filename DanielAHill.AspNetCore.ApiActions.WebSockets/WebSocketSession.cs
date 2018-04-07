using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public abstract class WebSocketSession : IWebSocketSession
    {
        private IWebSocketConnectionInitializationContext _context;

        public IWebSocketTunnel Socket => _context.Socket;

        public Task InitializeAsync(IWebSocketConnectionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            _context = initializationContext ?? throw new ArgumentNullException(nameof(initializationContext));

            // InitializeAsync Items
            return AppendedInitializeAsync(initializationContext, cancellationToken);
        }

        protected virtual Task AppendedInitializeAsync(IApiActionRouteInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public virtual Task<bool> AuthorizeAsync(HttpContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnCloseAsync(WebSocketCloseStatus status, string message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}