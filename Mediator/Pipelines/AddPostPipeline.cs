using Mediator.Commands.Add;
using MediatR.Pipeline;

namespace Mediator.Pipelines;

public class AddPostPipeline : IRequestPostProcessor<AddCommand, double>
{
    public Task Process(AddCommand request, double response, CancellationToken cancellationToken)
    {
        Console.WriteLine(response);
        return Task.CompletedTask;
    }
}
