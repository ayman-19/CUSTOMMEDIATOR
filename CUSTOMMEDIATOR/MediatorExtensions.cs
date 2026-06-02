using CUSTOMMEDIATOR.Implementations;
using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;

namespace CUSTOMMEDIATOR;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssemblies(typeof(MediatorExtensions).Assembly)
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddValidatorsFromAssembly(typeof(MediatorExtensions).Assembly);

        services.AddScoped<IMediator, Mediator>();

        services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationHandlerDecorator<,>));

        return services;
    }
}
