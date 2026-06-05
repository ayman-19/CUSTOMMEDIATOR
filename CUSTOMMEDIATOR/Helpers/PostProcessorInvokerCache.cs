using System.Collections.Concurrent;
using System.Linq.Expressions;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Helpers;

public static class PostProcessorInvokerCache<TResponse>
{
    public delegate Task PostProcessorInvoker(
        object processor,
        IRequest<TResponse> request,
        TResponse response,
        CancellationToken cancellationToken
    );

    private static readonly ConcurrentDictionary<Type, PostProcessorInvoker> _map = new();

    public static PostProcessorInvoker GetOrAdd(Type requestType, Type processorType) =>
        _map.TryGetValue(requestType, out var invoker)
            ? invoker
            : _map.GetOrAdd(requestType, static (rt, pt) => Compile(rt, pt), processorType);

    private static PostProcessorInvoker Compile(Type requestType, Type processorType)
    {
        var interfaceType = typeof(IRequestPostProcessor<,>).MakeGenericType(
            requestType,
            typeof(TResponse)
        );

        var method = interfaceType.GetMethod(nameof(IRequestPostProcessor<,>.Process))!;

        var processorParam = Expression.Parameter(typeof(object), "processor");
        var requestParam = Expression.Parameter(typeof(IRequest<TResponse>), "request");
        var responseParam = Expression.Parameter(typeof(TResponse), "response");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        var body = Expression.Call(
            Expression.Convert(processorParam, interfaceType),
            method,
            Expression.Convert(requestParam, requestType),
            responseParam,
            ctParam
        );

        return Expression
            .Lambda<PostProcessorInvoker>(
                body,
                processorParam,
                requestParam,
                responseParam,
                ctParam
            )
            .Compile();
    }
}
