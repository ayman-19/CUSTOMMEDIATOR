using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Commands.Add;

public sealed record AddCommand(double n1, double n2) : IRequest<double>;
