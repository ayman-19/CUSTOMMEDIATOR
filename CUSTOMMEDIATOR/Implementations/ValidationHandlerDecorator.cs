using CUSTOMMEDIATOR.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace CUSTOMMEDIATOR.Implementations;

//public class ValidationHandlerDecorator<TRequest, TResponse>(
//    IRequestHandler<TRequest, TResponse> innerHandler,
//    IEnumerable<IValidator<TRequest>> validators
//) : IRequestHandler<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    public async Task<TResponse> Handle(
//        TRequest request,
//        CancellationToken cancellationToken = default
//    )
//    {
//        if (validators != null && validators.Any())
//        {
//            var context = new ValidationContext<TRequest>(request);
//            List<ValidationFailure> failures = new();

//            foreach (var validator in validators)
//            {
//                var result = await validator.ValidateAsync(context, cancellationToken);
//                if (!result.IsValid)
//                    failures.AddRange(result.Errors);
//            }
//            if (failures.Any())
//                throw new ValidationException(failures);
//        }

//        return await innerHandler.Handle(request, cancellationToken);
//    }
//}


public class ValidationHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    IEnumerable<IValidator<TRequest>> validators,
    IEnumerable<IRequestPreProcessor<TRequest>> pipelines
//IEnumerable<IRequestPipeline<TRequest, TResponse>> pipelines
) : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var pipeline in pipelines)
            request = await pipeline.Process(request, cancellationToken);

        if (validators != null && validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            List<ValidationFailure> failures = [];

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }
            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await innerHandler.Handle(request, cancellationToken);
    }
}
