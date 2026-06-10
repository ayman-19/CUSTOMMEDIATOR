using System.Collections.Concurrent;
using System.Linq.Expressions;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Helpers;

public static class InvokerCache<TResponse>
{
    public delegate Task<TResponse> HandlerInvoker(
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
