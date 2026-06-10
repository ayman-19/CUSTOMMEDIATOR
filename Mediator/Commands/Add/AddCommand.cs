using MediatR;

namespace Mediator.Commands.Add;

public sealed record AddCommand : IRequest<double>
{
    public double n1 { get; set; }
    public double n2 { get; set; }
}
