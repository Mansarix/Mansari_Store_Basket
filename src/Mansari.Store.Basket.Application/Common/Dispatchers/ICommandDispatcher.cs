using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;

namespace Mansari.Store.Basket.Application.Common.Dispatchers;

public interface ICommandDispatcher
{
    Task<ServiceResult<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default);
}