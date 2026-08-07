namespace CUSTOMMEDIATOR.Interfaces;

public interface IMediator : IPublisher
{
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    );
}
