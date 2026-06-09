using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Pipelines;

public class AddPipeline : IRequestPreProcessor<AddCommand>
{
	public Task Process(AddCommand command, CancellationToken cancellationToken)
	{
		command.n1 += 1;
		command.n2 += 1;
		return Task.CompletedTask;
	}
}
