using Mansari.Store.Basket.API.Common.Controllers;
using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.API.Contracts;
using Mansari.Store.Basket.API.Contracts.Swagger;
using Mansari.Store.Basket.Application.Basket.Queries;
using Mansari.Store.Basket.Application.Common.Dispatchers;
using Mansari.Store.Basket.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Basket.API.Controllers;

/// <summary>
/// مدیریت عملیات سبد خرید کاربران.
/// </summary>
[Route("api/v1/basket")]
[Produces("application/json")]
public sealed class BasketController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher)
    : BaseController(commandDispatcher, queryDispatcher)
{
    /// <summary>
    /// دریافت سبد خرید فعال کاربر.
    /// </summary>
    /// <remarks>
    /// اگر کاربر هنوز سبد خرید فعالی نداشته باشد، یک سبد جدید برای او ایجاد و برگردانده می‌شود.
    /// </remarks>
    /// <param name="userId">شناسه کاربر.</param>
    /// <param name="cancellationToken">توکن لغو عملیات.</param>
    /// <returns>اطلاعات سبد خرید فعال کاربر.</returns>
    /// <response code="200">سبد خرید کاربر با موفقیت برگردانده شد.</response>
    /// <response code="400">درخواست نامعتبر است.</response>
    /// <response code="500">خطای داخلی سرور رخ داده است.</response>
    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(ApiResult<BasketSwaggerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetOrCreateBasket(
        long userId,
        CancellationToken cancellationToken)
    {
        return QueryAsync(
            new GetOrCreateBasketQuery(userId),
            cancellationToken);
    }

    /// <summary>
    /// افزودن کالا به سبد خرید.
    /// </summary>
    /// <remarks>
    /// در صورت وجود کالا در سبد، بسته به منطق دامنه، تعداد آن به‌روزرسانی می‌شود یا آیتم جدید ثبت می‌گردد.
    /// </remarks>
    /// <param name="userId">شناسه کاربر.</param>
    /// <param name="request">اطلاعات کالای مورد نظر برای افزودن به سبد خرید.</param>
    /// <param name="cancellationToken">توکن لغو عملیات.</param>
    /// <returns>سبد خرید به‌روزرسانی‌شده.</returns>
    /// <response code="200">کالا با موفقیت به سبد خرید اضافه شد.</response>
    /// <response code="400">داده‌های ارسالی نامعتبر است.</response>
    /// <response code="500">خطای داخلی سرور رخ داده است.</response>
    [HttpPost("{userId:long}/items")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResult<BasketSwaggerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    /// تغییر تعداد یک کالا در سبد خرید.
    /// </summary>
    /// <remarks>
    /// این عملیات فقط تعداد کالایی را تغییر می‌دهد که از قبل در سبد خرید کاربر وجود دارد.
    /// </remarks>
    /// <param name="userId">شناسه کاربر.</param>
    /// <param name="productId">شناسه محصول.</param>
    /// <param name="request">تعداد جدید کالا.</param>
    /// <param name="cancellationToken">توکن لغو عملیات.</param>
    /// <returns>سبد خرید به‌روزرسانی‌شده.</returns>
    /// <response code="200">تعداد کالا با موفقیت تغییر کرد.</response>
    /// <response code="400">داده‌های ارسالی نامعتبر است.</response>
    /// <response code="500">خطای داخلی سرور رخ داده است.</response>
    [HttpPut("{userId:long}/items/{productId:long}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResult<BasketSwaggerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    /// حذف یک کالا از سبد خرید.
    /// </summary>
    /// <param name="userId">شناسه کاربر.</param>
    /// <param name="productId">شناسه محصولی که باید از سبد خرید حذف شود.</param>
    /// <param name="cancellationToken">توکن لغو عملیات.</param>
    /// <returns>نتیجه عملیات حذف کالا.</returns>
    /// <response code="200">کالا با موفقیت از سبد خرید حذف شد.</response>
    /// <response code="400">درخواست نامعتبر است.</response>
    /// <response code="500">خطای داخلی سرور رخ داده است.</response>
    [HttpDelete("{userId:long}/items/{productId:long}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    /// حذف تمام کالاهای موجود در سبد خرید.
    /// </summary>
    /// <remarks>
    /// این عملیات سبد خرید کاربر را خالی می‌کند، اما خود سبد خرید را الزاماً حذف نمی‌کند.
    /// </remarks>
    /// <param name="userId">شناسه کاربر.</param>
    /// <param name="cancellationToken">توکن لغو عملیات.</param>
    /// <returns>نتیجه عملیات خالی کردن سبد خرید.</returns>
    /// <response code="200">سبد خرید با موفقیت خالی شد.</response>
    /// <response code="400">درخواست نامعتبر است.</response>
    /// <response code="500">خطای داخلی سرور رخ داده است.</response>
    [HttpDelete("{userId:long}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> ClearBasket(
        long userId,
        CancellationToken cancellationToken)
    {
        return CommandAsync(
            new ClearBasketCommand(userId),
            cancellationToken);
    }
}
