using FluentValidation;
using MediatR;

namespace Mediator;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (validators != null && validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            List<FluentValidation.Results.ValidationFailure> failures = new();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }
            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}
