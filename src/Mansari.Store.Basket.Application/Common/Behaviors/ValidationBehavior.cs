using FluentValidation;
using Mansari.Store.Basket.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Mansari.Store.Basket.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, ServiceResult<TResponse>>
    where TRequest : IRequest<ServiceResult<TResponse>>
{
    public async Task<ServiceResult<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<ServiceResult<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        return ServiceResult<TResponse>.Failure(
            "Validation Failed",
            StatusCodes.Status400BadRequest,
            failures.Select(x => x.ErrorMessage));
    }
}