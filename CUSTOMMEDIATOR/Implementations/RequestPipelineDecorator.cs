using CUSTOMMEDIATOR.Helpers;
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
    static readonly Type _requestType = typeof(TRequest);
    static readonly Type _processorType = typeof(IRequestPostProcessor<TRequest, TResponse>);

    readonly IValidator<TRequest>[] _validators = [.. validators];
    readonly IRequestPreProcessor<TRequest>[] _preProcessors = [.. preProcessors];
    readonly IRequestPostProcessor<TRequest, TResponse>[] _postProcessors = [.. postProcessors];

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        for (var i = 0; i < _preProcessors.Length; i++)
            await _preProcessors[i].Process(request, cancellationToken);

        if (_validators.Length > 0)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = new List<ValidationFailure>(_validators.Length);

            for (var i = 0; i < _validators.Length; i++)
            {
                var result = await _validators[i].ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }

        var response = await innerHandler.Handle(request, cancellationToken);

        if (_postProcessors.Length > 0)
        {
            var invoker = PostProcessorInvokerCache<TResponse>.GetOrAdd(
                _requestType,
                _processorType
            );

            for (var i = 0; i < _postProcessors.Length; i++)
                await invoker(_postProcessors[i], request, response, cancellationToken);
        }

        return response;
    }
}
