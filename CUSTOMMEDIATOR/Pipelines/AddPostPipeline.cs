using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Pipelines;

public class AddPostPipeline : IRequestPostProcessor<AddCommand, double>
{
    public Task Process(AddCommand request, double response, CancellationToken cancellationToken)
    {
        Console.WriteLine(response);
        return Task.CompletedTask;
    }
}
