namespace Mansari.Store.Basket.API.Common.Results;

public class ApiResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public IReadOnlyCollection<string>? Errors { get; init; }
}

// برای سوگر و توسعه بهتر در اینده
public sealed class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }
}