using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace CUSTOMMEDIATOR.Implementations;

public class RequestPipelineDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    IEnumerable<IValidator<TRequest>> validators,
    IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
    IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors
) : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    readonly IValidator<TRequest>[] _validators = [.. validators];
    readonly IRequestPreProcessor<TRequest>[] _preProcessors = [.. preProcessors];
    readonly IRequestPostProcessor<TRequest, TResponse>[] _postProcessors = [.. postProcessors];

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        for (int i = 0; i < _preProcessors.Length; i++)
            await _preProcessors[i].Process(request, cancellationToken);

        if (_validators.Length > 0)
        {
            ValidationContext<TRequest> context = new(request);
            List<ValidationFailure> failures = new(_validators.Length);

            for (int i = 0; i < _validators.Length; i++)
            {
                ValidationResult result = await _validators[i]
                    .ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }

        var response = await innerHandler.Handle(request, cancellationToken);

        for (int i = 0; i < _postProcessors.Length; i++)
            await _postProcessors[i].Process(request, response, cancellationToken);

        return response;
    }
}
