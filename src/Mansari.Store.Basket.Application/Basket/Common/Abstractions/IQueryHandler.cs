using Mansari.Store.Basket.Application.Common.Results;
using MediatR;

namespace Mansari.Store.Basket.Application.Common.Abstractions;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, ServiceResult<TResponse>>
    where TQuery : IQuery<TResponse>
{
}