using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using MediatR;

namespace Mansari.Store.Basket.Application.Basket.Common.Abstractions;

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, ServiceResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
}