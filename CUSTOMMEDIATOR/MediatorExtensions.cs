using System.Reflection;
using CUSTOMMEDIATOR.Implementations;
using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;

namespace CUSTOMMEDIATOR;

public static class MediatorExtensions
{
    //public static IServiceCollection AddMediator(
    //    this IServiceCollection services,
    //    Assembly assembly
    //)
    //{
    //    services.AddScoped<IMediator, Mediator>();
    //    RegisterHandlers(services, assembly);
    //    services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    //    //services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();
    //    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    //    return services;
    //}

    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        RegisterHandlers(services, assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IMediator, Mediator>();

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var requestHandlerTypes = assembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    )
            )
            .ToList();

        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType
                .GetInterfaces()
                .First(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                );

            services.AddTransient(handlerInterface, handlerType);
        }

        var notificationHandlerTypes = assembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )
            )
            .ToList();

        foreach (var handlerType in notificationHandlerTypes)
        {
            var handlerInterfaces = handlerType
                .GetInterfaces()
                .Where(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                );

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddTransient(handlerInterface, handlerType);
            }
        }
    }
}
