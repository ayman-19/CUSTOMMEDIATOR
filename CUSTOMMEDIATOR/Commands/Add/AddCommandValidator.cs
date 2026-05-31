using FluentValidation;

namespace CUSTOMMEDIATOR.Commands.Add;

public class AddCommandValidator : AbstractValidator<AddCommand>
{
    public AddCommandValidator()
    {
        RuleFor(x => x.n1).GreaterThan(0);

        RuleFor(x => x.n2).GreaterThan(0);
    }
}
