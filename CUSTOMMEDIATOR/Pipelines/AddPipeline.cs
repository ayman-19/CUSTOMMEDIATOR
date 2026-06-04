using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Pipelines;

//public class AddPipeline : IRequestPipeline<AddCommand, double>
//{
//    public Task<AddCommand> Process(
//        AddCommand command,
//        CancellationToken cancellationToken = default
//    )
//    {
//        return Task.FromResult(command with { n1 = command.n1 + 1, n2 = command.n2 + 1 });
//    }
//}

public class AddPipeline : IRequestPreProcessor<AddCommand>
{
    public Task<AddCommand> Process(
        AddCommand command,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(command with { n1 = command.n1 + 1, n2 = command.n2 + 1 });
    }
}
