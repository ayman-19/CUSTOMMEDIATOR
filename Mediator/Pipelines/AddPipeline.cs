using Mediator.Commands.Add;
using MediatR.Pipeline;

namespace Mediator.Pipelines;

public sealed class AddPipeline : IRequestPreProcessor<AddCommand>
{
    public Task Process(AddCommand command, CancellationToken cancellationToken)
    {
        command.n1 += 1;
        command.n2 += 1;
        return Task.CompletedTask;
    }
}
