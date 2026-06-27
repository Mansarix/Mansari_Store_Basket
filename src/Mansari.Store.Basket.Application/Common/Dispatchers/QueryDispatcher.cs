using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using MediatR;

namespace Mansari.Store.Basket.Application.Common.Dispatchers;

public sealed class QueryDispatcher(
    IMediator mediator)
    : IQueryDispatcher
{
    public Task<ServiceResult<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        return mediator.Send(
            query,
            cancellationToken);
    }
}