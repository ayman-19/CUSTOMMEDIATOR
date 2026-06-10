using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace CUSTOMMEDIATOR.Pipelines;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    readonly IValidator<TRequest>[] _validators = [.. validators];

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validators.Length == 0)
            return await next(cancellationToken);
        var context = new ValidationContext<TRequest>(request);
        var failures = new List<ValidationFailure>(_validators.Length);
        for (int i = 0; i < _validators.Length; i++)
        {
            ValidationResult result = await _validators[i]
                .ValidateAsync(context, cancellationToken);
            if (!result.IsValid)
                failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
            throw new ValidationException(failures);
        return await next(cancellationToken);
    }
}
