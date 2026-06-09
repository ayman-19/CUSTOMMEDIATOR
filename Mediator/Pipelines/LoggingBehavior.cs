using MediatR;
using System.Diagnostics;

namespace Mediator.Pipelines;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken
	)
	{
		var timer = Stopwatch.StartNew();

		var response = await next(cancellationToken);

		timer.Stop();
		Console.WriteLine(
			$"طلب {typeof(TRequest).Name} استغرق {timer.ElapsedMilliseconds} ملي ثانية."
		);

		return response;
	}
}
