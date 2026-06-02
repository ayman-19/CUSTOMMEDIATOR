using System.Collections.Concurrent;
using System.Linq.Expressions;
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

    private static class HandlerCache<TResponse>
    {
        private static readonly ConcurrentDictionary<Type, Type> _map = new();

        public static Type HandlerType(Type requestType) =>
            _map.GetOrAdd(
                requestType,
                static t => typeof(IRequestHandler<,>).MakeGenericType(t, typeof(TResponse))
            );
    }

    private static class InvokerCache<TResponse>
    {
        internal delegate Task<TResponse> HandlerInvoker(
            object handler,
            IRequest<TResponse> request,
            CancellationToken ct
        );

        private static readonly ConcurrentDictionary<Type, HandlerInvoker> _map = new();

        public static HandlerInvoker GetOrAdd(Type requestType) =>
            _map.TryGetValue(requestType, out var invoker)
                ? invoker
                : _map.GetOrAdd(requestType, Compile);

        private static HandlerInvoker Compile(Type requestType)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(
                requestType,
                typeof(TResponse)
            );

            var method = handlerType.GetMethod(nameof(IRequestHandler<,>.Handle))!;

            var handler = Expression.Parameter(typeof(object), "handler");

            var request = Expression.Parameter(typeof(IRequest<TResponse>), "request");

            var cancellationToken = Expression.Parameter(typeof(CancellationToken), "ct");

            var body = Expression.Call(
                Expression.Convert(handler, handlerType),
                method,
                Expression.Convert(request, requestType),
                cancellationToken
            );

            return Expression
                .Lambda<HandlerInvoker>(body, handler, request, cancellationToken)
                .Compile();
        }
    }
}
