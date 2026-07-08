using DocAnalytics.Api.Common;
using DocAnalytics.Service.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/files")]
public sealed class UploadsController : ControllerBase
{
    private readonly IUploadService _uploads;
    public UploadsController(IUploadService uploads) => _uploads = uploads;

    // POST /api/v1/files/upload-url
    [HttpPost("upload-url")]
    public async Task<IActionResult> GetUploadUrl([FromBody] UploadUrlRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _uploads.CreateUploadAsync(req, ct);
            return Ok(ApiResponse<UploadUrlResponse>.Ok(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail("UPLOAD_REJECTED", ex.Message));
        }
    }

    // POST /api/v1/files/{id}/complete
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.CompleteAsync(id, ct);
        return ok
            ? Ok(ApiResponse<object>.Ok(new { queued = true }))
            : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "File not found."));
    }
}
