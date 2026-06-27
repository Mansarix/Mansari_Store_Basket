using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;

namespace Mansari.Store.Basket.Application.Common.Dispatchers;

public interface IQueryDispatcher
{
    Task<ServiceResult<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default);
}