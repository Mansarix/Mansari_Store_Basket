using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Basket.API.Common.Extensions;

public static class ServiceResultExtensions
{
    public static IActionResult ToApiResult<T>(
        this ServiceResult<T> result)
    {
        var response = new ApiResult<T>
        {
            Success = result.IsSuccess,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors
        };

        return new ObjectResult(response)
        {
            StatusCode = result.StatusCode
        };
    }

    public static IActionResult ToApiResult(
        this ServiceResult result)
    {
        var response = new ApiResult<object?>
        {
            Success = result.IsSuccess,
            Message = result.Message,
            Errors = result.Errors
        };

        return new ObjectResult(response)
        {
            StatusCode = result.StatusCode
        };
    }
}