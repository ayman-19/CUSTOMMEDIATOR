using System.Reflection;
using CUSTOMMEDIATOR.Implementations;
using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;

namespace CUSTOMMEDIATOR;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        services.AddScoped<IMediator, Mediator>();

        services.AddValidatorsFromAssembly(assembly);
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
        services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationHandlerDecorator<,>));

        return services;
    }
}
