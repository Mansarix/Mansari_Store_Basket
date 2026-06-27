using Microsoft.AspNetCore.Http;

namespace Mansari.Store.Basket.Application.Common.Results;

public class ServiceResult
{
    public bool IsSuccess { get; init; }

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public IReadOnlyCollection<string>? Errors { get; init; }

    public static ServiceResult Success(
        string message = "")
        => new()
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message
        };

    public static ServiceResult Failure(
        string message,
        int statusCode,
        IEnumerable<string>? errors = null)
        => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList()
        };
}

public sealed class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Success(
        T data,
        string message = "")
        => new()
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = message,
            Data = data
        };

    public new static ServiceResult<T> Failure(
        string message,
        int statusCode,
        IEnumerable<string>? errors = null)
        => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList()
        };
}