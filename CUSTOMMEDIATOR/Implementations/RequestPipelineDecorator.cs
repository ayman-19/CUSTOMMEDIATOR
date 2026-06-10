using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR.Implementations;

public class RequestPipelineDecorator<TRequest, TResponse>(
	IRequestHandler<TRequest, TResponse> innerHandler,
	IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
	IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors,
	IEnumerable<IPipelineBehavior<TRequest, TResponse>> pipelines
) : IRequestHandler<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	readonly IRequestPreProcessor<TRequest>[] _preProcessors = [.. preProcessors];
	readonly IRequestPostProcessor<TRequest, TResponse>[] _postProcessors = [.. postProcessors];
	readonly IPipelineBehavior<TRequest, TResponse>[] _pipelines = [.. pipelines];

	public async Task<TResponse> Handle(
		TRequest request,
		CancellationToken cancellationToken = default
	)
	{
		for (int i = 0; i < _preProcessors.Length; i++)
			await _preProcessors[i].Process(request, cancellationToken);

		RequestHandlerDelegate<TResponse> next = async (ct) =>
		{
			var response = await innerHandler.Handle(request, ct);

			for (int i = 0; i < _postProcessors.Length; i++)
				await _postProcessors[i].Process(request, response, ct);

			return response;
		};

		for (int i = _pipelines.Length - 1; i >= 0; i--)
		{
			var pipeline = _pipelines[i];
			var currentNext = next;
			next = (ct) => pipeline.Handle(request, currentNext, ct);
		}

		return await next(cancellationToken);
	}
}
