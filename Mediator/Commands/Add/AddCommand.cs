using MediatR;

namespace Mediator.Commands.Add;

public sealed record AddCommand(double n1, double n2) : IRequest<double>;
