using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Commands.Add;

public sealed class AddCommandHandler : IRequestHandler<AddCommand, double>
{
    public Task<double> Handle(AddCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(command.n1 + command.n2);
    }
}
