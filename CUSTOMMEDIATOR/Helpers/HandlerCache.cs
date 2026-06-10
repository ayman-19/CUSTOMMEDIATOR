using System.Collections.Concurrent;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Helpers;

public static class HandlerCache<TResponse>
{
    private static readonly ConcurrentDictionary<Type, Type> _map = new();

    public static Type HandlerType(Type requestType) =>
        _map.GetOrAdd(
            requestType,
            static t => typeof(IRequestHandler<,>).MakeGenericType(t, typeof(TResponse))
        );
}
