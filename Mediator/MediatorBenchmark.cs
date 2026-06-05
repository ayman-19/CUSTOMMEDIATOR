using BenchmarkDotNet.Attributes;
using CUSTOMMEDIATOR;
using FluentValidation;
using Mediator.Commands.Add;
using Mediator.Pipelines;
using MediatR;
using MediatR.Pipeline;

namespace Mediator;

[MemoryDiagnoser]
[SimpleJob]
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
}
