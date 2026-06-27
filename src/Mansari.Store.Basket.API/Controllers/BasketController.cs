using Mansari.Store.Basket.API.Common.Controllers;
using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.API.Contracts;
using Mansari.Store.Basket.Application.Basket.Queries;
using Mansari.Store.Basket.Application.Common.Dispatchers;
using Mansari.Store.Basket.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Basket.API.Controllers;

/// <summary>
/// مدیریت عملیات سبد خرید کاربران
/// </summary>
[Route("api/v1/basket")]
public sealed class BasketController(
ICommandDispatcher commandDispatcher,
IQueryDispatcher queryDispatcher)
: BaseController(
commandDispatcher,
queryDispatcher)
{
    /// <summary>
    /// دریافت سبد فعال کاربر یا ایجاد سبد جدید
    /// </summary>
    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(ApiResult<BasketDTO>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetOrCreateBasket(
    long userId,
    CancellationToken cancellationToken)
    {
        return QueryAsync(
        new GetOrCreateBasketQuery(userId),
        cancellationToken);
    }

/// <summary>
/// افزودن کالا به سبد خرید
/// </summary>
[HttpPost("{userId:long}/items")]
    [ProducesResponseType(typeof(ApiResult<BasketDTO>), StatusCodes.Status200OK)]
    public Task<IActionResult> AddItem(
    long userId,
    [FromBody] AddBasketItemDTO request,
    CancellationToken cancellationToken)
    {
        return CommandAsync(
            new AddItemToBasketCommand(
                userId,
                request),
            cancellationToken);
    }

    /// <summary>
    /// تغییر تعداد یک کالای موجود در سبد
    /// </summary>
    [HttpPut("{userId:long}/items/{productId:long}")]
    [ProducesResponseType(typeof(ApiResult<BasketDTO>), StatusCodes.Status200OK)]
    public Task<IActionResult> UpdateQuantity(
        long userId,
        long productId,
        [FromBody] UpdateBasketItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        return CommandAsync(
            new UpdateBasketItemQuantityCommand(
                userId,
                productId,
                request.Quantity),
            cancellationToken);
    }

    /// <summary>
    /// حذف یک کالا از سبد خرید
    /// </summary>
    [HttpDelete("{userId:long}/items/{productId:long}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    public Task<IActionResult> RemoveItem(
        long userId,
        long productId,
        CancellationToken cancellationToken)
    {
        return CommandAsync(
            new RemoveBasketItemCommand(
                userId,
                productId),
            cancellationToken);
    }

    /// <summary>
    /// حذف تمام آیتم‌های سبد خرید
    /// </summary>
    [HttpDelete("{userId:long}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    public Task<IActionResult> ClearBasket(
        long userId,
        CancellationToken cancellationToken)
    {
        return CommandAsync(
            new ClearBasketCommand(userId),
            cancellationToken);
    }

}
