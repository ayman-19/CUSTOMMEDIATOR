using BenchmarkDotNet.Attributes;
using CUSTOMMEDIATOR;
using FluentValidation;
using Mediator.Commands.Add;
using MediatR;

namespace Mediator;

[MemoryDiagnoser]
[SimpleJob]
public class MediatorBenchmark
{
    private IMediator _mediator = null!;
    CUSTOMMEDIATOR.Interfaces.IMediator _customMediator = null!;
    private AddCommand _command = null!;

    [GlobalSetup]
    public void Setup()
    {
        _command = new AddCommand(10.5, 20.3);

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddCommandHandler>());
        services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        MediatorExtensions.AddMediator(services);

        var provider = services.BuildServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();

        _customMediator = provider.GetRequiredService<CUSTOMMEDIATOR.Interfaces.IMediator>();
    }

    [Benchmark]
    public Task<double> MediatorSend() => _mediator.Send(_command);

    [Benchmark]
    public Task<double> CustomMediatorSend() =>
        _customMediator.Send(new CUSTOMMEDIATOR.Commands.Add.AddCommand(10.5, 20.3));
}
