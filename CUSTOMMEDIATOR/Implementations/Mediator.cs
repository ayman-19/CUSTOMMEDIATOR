using CUSTOMMEDIATOR.Helpers;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Implementations;

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = serviceProvider.GetRequiredService(
            HandlerCache<TResponse>.HandlerType(request.GetType())
        );

        return InvokerCache<TResponse>
            .GetOrAdd(request.GetType())
            .Invoke(handler, request, cancellationToken);
    }
}
