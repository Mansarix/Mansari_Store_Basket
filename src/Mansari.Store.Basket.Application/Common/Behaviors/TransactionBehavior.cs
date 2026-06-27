using Mansari.Store.Basket.Application.Basket.Common.Dispatchers;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mansari.Store.Basket.Application.Common.Behaviors;

/// <summary>
/// مدیریت تراکنش برای تمام Command ها
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher)
    : IPipelineBehavior<TRequest, ServiceResult<TResponse>>
    where TRequest : IRequest<ServiceResult<TResponse>>
{
    public async Task<ServiceResult<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<ServiceResult<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>)
        {
            return await next();
        }

        await unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            var result = await next();

            if (!result.IsSuccess)
            {
                await unitOfWork.RollbackAsync(
                    cancellationToken);

                return result;
            }

            await domainEventDispatcher.DispatchAsync(
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            await unitOfWork.CommitAsync(
                cancellationToken);

            return result;
        }
        catch
        {
            await unitOfWork.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}