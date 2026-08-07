using MediatR;

namespace Mediator.Notifications;

public class PingNotification1 : INotification { }

public class PingNotification5 : INotification { }

public class PingNotification10 : INotification { }

public class Ping1Handler : INotificationHandler<PingNotification1>
{
    public Task Handle(PingNotification1 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping5Handler1 : INotificationHandler<PingNotification5>
{
    public Task Handle(PingNotification5 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping5Handler2 : INotificationHandler<PingNotification5>
{
    public Task Handle(PingNotification5 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping5Handler3 : INotificationHandler<PingNotification5>
{
    public Task Handle(PingNotification5 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping5Handler4 : INotificationHandler<PingNotification5>
{
    public Task Handle(PingNotification5 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping5Handler5 : INotificationHandler<PingNotification5>
{
    public Task Handle(PingNotification5 notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class Ping10Handler1 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler2 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler3 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler4 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler5 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler6 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler7 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler8 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler9 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}

public class Ping10Handler10 : INotificationHandler<PingNotification10>
{
    public Task Handle(
        PingNotification10 notification,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine(10);
        return Task.CompletedTask;
    }
}
