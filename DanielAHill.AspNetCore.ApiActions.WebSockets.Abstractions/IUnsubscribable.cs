using System;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.WebSockets
{
    public interface IUnsubscribable
    {
        Guid Id { get; }
        Task OnUnsubscribeAsync(CancellationToken cancellationToken);
    }
}