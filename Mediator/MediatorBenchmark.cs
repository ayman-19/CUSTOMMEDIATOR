using BenchmarkDotNet.Attributes;
using CUSTOMMEDIATOR;
using FluentValidation;
using Mediator.Commands.Add;
using Mediator.Pipelines;
using MediatR;
using MediatR.Pipeline;

namespace Mediator;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 5)]
public class MediatorBenchmark
{
    private IMediator _mediator = null!;
    CUSTOMMEDIATOR.Interfaces.IMediator _customMediator = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddCommandHandler>();
            cfg.AddOpenBehavior(typeof(RequestPostProcessorBehavior<,>));
            cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
        });
        services.AddTransient<IRequestPostProcessor<AddCommand, double>, AddPostPipeline>();
        services.AddScoped<IRequestPreProcessor<AddCommand>, AddPipeline>();
        services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        MediatorExtensions.AddMediator(services);

        var provider = services.BuildServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();

        _customMediator = provider.GetRequiredService<CUSTOMMEDIATOR.Interfaces.IMediator>();
    }

    [Benchmark]
    public Task<double> MediatorSend() => _mediator.Send(new AddCommand { n1 = 10.5, n2 = 20.3 });

    [Benchmark]
    public Task<double> CustomMediatorSend() =>
        _customMediator.Send(new CUSTOMMEDIATOR.Commands.Add.AddCommand { n1 = 10.5, n2 = 20.3 });

    [Benchmark]
    public Task MediatorPublish1() =>
        _mediator.Publish(new Mediator.Notifications.PingNotification1());

    [Benchmark]
    public Task MediatorPublish5() =>
        _mediator.Publish(new Mediator.Notifications.PingNotification5());

    [Benchmark]
    public Task MediatorPublish10() =>
        _mediator.Publish(new Mediator.Notifications.PingNotification10());

    [Benchmark]
    public Task CustomMediatorPublish1() =>
        _customMediator.Publish(new CUSTOMMEDIATOR.Notifications.PingNotification1());

    [Benchmark]
    public Task CustomMediatorPublish5() =>
        _customMediator.Publish(new CUSTOMMEDIATOR.Notifications.PingNotification5());

    [Benchmark]
    public Task CustomMediatorPublish10() =>
        _customMediator.Publish(new CUSTOMMEDIATOR.Notifications.PingNotification10());
}
