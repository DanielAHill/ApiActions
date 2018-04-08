using System;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IUnsubscribable
    {
        string CommandId { get; }
        Task OnUnsubscribeAsync(CancellationToken cancellationToken);
    }
}