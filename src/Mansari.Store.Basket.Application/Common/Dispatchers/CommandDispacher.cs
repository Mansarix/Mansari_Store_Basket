using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using MediatR;

namespace Mansari.Store.Basket.Application.Common.Dispatchers;

public sealed class CommandDispatcher(
    IMediator mediator)
    : ICommandDispatcher
{
    public Task<ServiceResult<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        return mediator.Send(
            command,
            cancellationToken);
    }
}