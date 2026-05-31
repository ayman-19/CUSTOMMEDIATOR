using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Implementations;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(
            requestType,
            typeof(TResponse)
        );

        var handler = serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException(
                $"No handler registered for request type {requestType.Name}"
            );
        }

        var method = handlerType.GetMethod(
            nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle)
        );
        var task = (Task<TResponse>)method!.Invoke(handler, [request, cancellationToken])!;

        return await task;
    }
}
