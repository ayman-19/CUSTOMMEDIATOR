using CUSTOMMEDIATOR.Implementations;
using CUSTOMMEDIATOR.Interfaces;
using CUSTOMMEDIATOR.Pipelines;
using FluentValidation;

namespace CUSTOMMEDIATOR;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        // Handlers
        services.Scan(scan =>
            scan.FromAssemblies(typeof(MediatorExtensions).Assembly)
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        //Pre Processor Pipelines
        services.Scan(scan =>
            scan.FromAssemblies(typeof(MediatorExtensions).Assembly)
                .AddClasses(c => c.AssignableTo(typeof(IRequestPreProcessor<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        //Post Processor Pipelines
        services.Scan(scan =>
            scan.FromAssemblies(typeof(MediatorExtensions).Assembly)
                .AddClasses(
                    c => c.AssignableTo(typeof(IRequestPostProcessor<,>)),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddValidatorsFromAssembly(typeof(MediatorExtensions).Assembly);

        services.AddScoped<IMediator, Mediator>();

        services.Decorate(typeof(IRequestHandler<,>), typeof(RequestPipelineDecorator<,>));

        return services;
    }
}
