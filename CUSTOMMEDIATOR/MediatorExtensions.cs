using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Implementations;
using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;

namespace CUSTOMMEDIATOR;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        var assembly = typeof(AddCommandHandler).Assembly;

        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();

        services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationHandlerDecorator<,>));

        return services;
    }
}
