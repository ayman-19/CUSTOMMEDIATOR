namespace CUSTOMMEDIATOR.Interfaces;

public interface IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken
	);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(
	CancellationToken cancellationToken = default
);
