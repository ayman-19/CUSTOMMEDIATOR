using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Implementations;

public class Mediator(IServiceProvider _serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(
            requestType,
            typeof(TResponse)
        );

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(
            requestType,
            typeof(TResponse)
        );

        var behaviors = _serviceProvider.GetServices(behaviorType).Cast<object>();

        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var method = handlerType.GetMethod(
                nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle)
            );
            return (Task<TResponse>)
                method.Invoke(handler, new object[] { request, cancellationToken });
        };

        foreach (var behavior in behaviors.Reverse())
        {
            var currentPipeline = pipeline;
            var behaviorHandleMethod = behavior
                .GetType()
                .GetMethod(nameof(IPipelineBehavior<,>.Handle));

            pipeline = () =>
                (Task<TResponse>)
                    behaviorHandleMethod.Invoke(
                        behavior,
                        new object[] { request, currentPipeline, cancellationToken }
                    );
        }

        return await pipeline();
    }

    public async Task<TResponse> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResponse>
    {
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
        if (handler == null)
            throw new InvalidOperationException(
                $"No handler registered for {typeof(TRequest).Name}"
            );

        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        RequestHandlerDelegate<TResponse> pipeline = () =>
            handler.Handle(request, cancellationToken);

        foreach (var behavior in behaviors.Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }

    public async Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default
    )
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle");
            tasks.Add(
                (Task)method.Invoke(handler, new object[] { notification, cancellationToken })
            );
        }

        await Task.WhenAll(tasks);
    }
}


//public class Mediator : IMediator
//{
//    private readonly IServiceProvider _provider;

//    public Mediator(IServiceProvider provider)
//    {
//        _provider = provider;
//    }

//    public Task Publish<TNotification>(
//        TNotification notification,
//        CancellationToken cancellationToken = default
//    )
//        where TNotification : INotification
//    {
//        throw new NotImplementedException();
//    }

//    public async Task<TResponse> Send<TResponse>(
//        IRequest<TResponse> request,
//        CancellationToken ct = default
//    )
//    {
//        // Build the handler type dynamically
//        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(
//            request.GetType(),
//            typeof(TResponse)
//        );

//        // Resolve from DI
//        var handler = _provider.GetRequiredService(handlerType);

//        // Invoke HandleAsync via reflection
//        var method = handlerType.GetMethod(
//            nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle)
//        )!;

//        var task = (Task<TResponse>)method.Invoke(handler, new object[] { request, ct })!;

//        return await task;
//    }
//}
