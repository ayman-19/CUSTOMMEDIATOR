namespace CUSTOMMEDIATOR.Interfaces;

//public interface IRequestPipeline<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    Task<TRequest> Process(TRequest request, CancellationToken cancellationToken = default);
//}

public interface IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    Task<TRequest> Process(TRequest request, CancellationToken cancellationToken = default);
}
