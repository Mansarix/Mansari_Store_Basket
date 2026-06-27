using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.Domain.Common;

namespace Mansari.Store.Basket.API.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            context.Response.StatusCode = 400;

            await context.Response.WriteAsJsonAsync(
                new ApiResult<object>
                {
                    Success = false,
                    Message = ex.Message
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(
                new ApiResult<object>
                {
                    Success = false,
                    Message = "خطای داخلی سرور"
                });
        }
    }
}