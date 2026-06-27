using Mansari.Store.Basket.API.Common.Extensions;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Dispatchers;
using Mansari.Store.Basket.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Basket.API.Common.Controllers;

/// <summary>
/// کنترلر پایه جهت یکپارچه سازی خروجی تمام API ها
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected readonly ICommandDispatcher CommandDispatcher;
    protected readonly IQueryDispatcher QueryDispatcher;

    protected BaseController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        CommandDispatcher = commandDispatcher;
        QueryDispatcher = queryDispatcher;
    }

    protected async Task<IActionResult> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        var result = await QueryDispatcher.QueryAsync(
            query,
            cancellationToken);

        return result.ToApiResult();
    }

    protected async Task<IActionResult> CommandAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        var result = await CommandDispatcher.SendAsync(
            command,
            cancellationToken);

        return result.ToApiResult();
    }
}