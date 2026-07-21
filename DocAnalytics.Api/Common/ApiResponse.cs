namespace DocAnalytics.Api.Common;

/// <summary>Standard response envelope wrapping a payload, optional paging metadata, and an optional error.</summary>
/// <typeparam name="T">The payload type.</typeparam>
public class ApiResponse<T>
{
    /// <summary>The payload, when the request succeeded.</summary>
    public T? Data { get; set; }
    /// <summary>Paging metadata, for list responses.</summary>
    public Meta? Meta { get; set; }
    /// <summary>Error details, when the request failed.</summary>
    public ApiError? Error { get; set; }

    /// <summary>Creates a success envelope for a single payload.</summary>
    /// <param name="data">The payload.</param>
    /// <returns>The envelope.</returns>
    public static ApiResponse<T> Ok(T data) => new() { Data = data };

    /// <summary>Creates a success envelope for a list payload with paging metadata.</summary>
    /// <param name="data">The payload.</param>
    /// <param name="meta">The paging metadata.</param>
    /// <returns>The envelope.</returns>
    public static ApiResponse<T> OkList(T data, Meta meta) => new() { Data = data, Meta = meta };

    /// <summary>Creates a failure envelope with an error code, message, and optional details.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="msg">The human-readable message.</param>
    /// <param name="details">Optional structured error details.</param>
    /// <returns>The envelope.</returns>
    public static ApiResponse<T> Fail(string code, string msg, object? details = null)
        => new() { Error = new ApiError { Code = code, Message = msg, Details = details } };
}

/// <summary>Paging metadata for list responses.</summary>
public class Meta
{
    /// <summary>Total rows across all pages.</summary>
    public int TotalCount { get; set; }
    /// <summary>The 1-based page number.</summary>
    public int Page { get; set; }
    /// <summary>Rows per page.</summary>
    public int PageSize { get; set; }
    /// <summary>Total number of pages.</summary>
    public int TotalPages { get; set; }
}

/// <summary>Structured error information returned in a failure envelope.</summary>
public class ApiError
{
    /// <summary>Machine-readable error code.</summary>
    public string Code { get; set; } = null!;
    /// <summary>Human-readable error message.</summary>
    public string Message { get; set; } = null!;
    /// <summary>Optional structured details (e.g. validation errors).</summary>
    public object? Details { get; set; }
}
