using System.Collections.Concurrent;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Helpers;

public static class NotificationInvokerCache
{
    public delegate Task NotificationInvoker(
        IServiceProvider serviceProvider,
        INotification notification,
        CancellationToken cancellationToken
    );

    private static readonly ConcurrentDictionary<Type, NotificationInvoker> _map = new();

    public static NotificationInvoker GetOrAdd(Type notificationType) =>
        _map.TryGetValue(notificationType, out var invoker)
            ? invoker
            : _map.GetOrAdd(notificationType, CreateInvoker(notificationType));

    private static NotificationInvoker CreateInvoker(Type notificationType)
    {
        var wrapperType = typeof(NotificationWrapper<>).MakeGenericType(notificationType);
        var wrapper = (INotificationWrapper)Activator.CreateInstance(wrapperType)!;
        return wrapper.Handle;
    }

    private interface INotificationWrapper
    {
        Task Handle(
            IServiceProvider serviceProvider,
            INotification notification,
            CancellationToken cancellationToken
        );
    }

    private sealed class NotificationWrapper<TNotification> : INotificationWrapper
        where TNotification : INotification
    {
        public Task Handle(
            IServiceProvider serviceProvider,
            INotification notification,
            CancellationToken cancellationToken
        )
        {
            var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();

            if (handlers is INotificationHandler<TNotification>[] array)
            {
                var count = array.Length;
                if (count == 0)
                    return Task.CompletedTask;
                if (count == 1)
                    return array[0].Handle((TNotification)notification, cancellationToken);

                var tasks = new Task[count];
                for (int i = 0; i < count; i++)
                {
                    tasks[i] = array[i].Handle((TNotification)notification, cancellationToken);
                }
                return Task.WhenAll(tasks);
            }

            var list = handlers.ToList();
            if (list.Count == 0)
                return Task.CompletedTask;
            if (list.Count == 1)
                return list[0].Handle((TNotification)notification, cancellationToken);

            var tasksList = new Task[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                tasksList[i] = list[i].Handle((TNotification)notification, cancellationToken);
            }
            return Task.WhenAll(tasksList);
        }
    }
}
