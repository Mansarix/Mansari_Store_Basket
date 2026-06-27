using Mansari.Store.Basket.Application.Common.Results;
using MediatR;

namespace Mansari.Store.Basket.Application.Common.Abstractions;

public interface IQuery<TResponse>
    : IRequest<ServiceResult<TResponse>>
{
}