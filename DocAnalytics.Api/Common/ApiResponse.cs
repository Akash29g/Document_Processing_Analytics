namespace DocAnalytics.Api.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public Meta? Meta { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Data = data };
    public static ApiResponse<T> OkList(T data, Meta meta) => new() { Data = data, Meta = meta };
    public static ApiResponse<T> Fail(string code, string msg, object? details = null)
        => new() { Error = new ApiError { Code = code, Message = msg, Details = details } };
}
public class Meta { public int TotalCount { get; set; } public int Page { get; set; } public int PageSize { get; set; } public int TotalPages { get; set; } }
public class ApiError { public string Code { get; set; } = null!; public string Message { get; set; } = null!; public object? Details { get; set; } }
