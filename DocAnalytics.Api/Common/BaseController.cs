using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Common;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Envelope<T>(T data) => Ok(ApiResponse<T>.Ok(data));
    protected IActionResult EnvelopeList<T>(T data, Meta meta) => Ok(ApiResponse<T>.OkList(data, meta));
}
