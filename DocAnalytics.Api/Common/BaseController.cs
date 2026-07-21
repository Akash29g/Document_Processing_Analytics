using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Common;

/// <summary>Base API controller providing helpers to wrap results in the standard <see cref="ApiResponse{T}"/> envelope.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>Wraps a single payload in a success envelope.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="data">The payload.</param>
    /// <returns>A 200 OK result with the wrapped payload.</returns>
    protected IActionResult Envelope<T>(T data) => Ok(ApiResponse<T>.Ok(data));

    /// <summary>Wraps a list payload plus paging metadata in a success envelope.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="data">The payload.</param>
    /// <param name="meta">The paging metadata.</param>
    /// <returns>A 200 OK result with the wrapped payload and metadata.</returns>
    protected IActionResult EnvelopeList<T>(T data, Meta meta) => Ok(ApiResponse<T>.OkList(data, meta));
}
