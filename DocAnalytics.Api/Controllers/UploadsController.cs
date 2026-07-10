using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DocAnalytics.Api.Controllers;

[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
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
        catch (DuplicateFileException ex)
        {
            return Conflict(ApiResponse<object>.Fail("DUPLICATE_FILE", ex.Message));
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

    // POST /api/v1/files/batches
    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _uploads.CreateBatchAsync(req, ct);
            return Ok(ApiResponse<CreateBatchResponse>.Ok(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail("BATCH_REJECTED", ex.Message));
        }
    }

    // POST /api/v1/files/batches/{id}/shrink — a planned file was skipped
    [HttpPost("batches/{id:guid}/shrink")]
    public async Task<IActionResult> ShrinkBatch(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.ShrinkBatchAsync(id, ct);
        return ok ? Ok(ApiResponse<object>.Ok(new { shrunk = true }))
                  : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Batch not found."));
    }

    // DELETE /api/v1/files/batches/{id} — admin only
    [HttpDelete("batches/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBatch(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.DeleteBatchAsync(id, ct);
        return ok ? Ok(ApiResponse<object>.Ok(new { deleted = true }))
                  : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Batch not found."));
    }

    // GET /api/v1/files/{id}/download-url — short-lived presigned S3 GET
    [HttpGet("{id:guid}/download-url")]
    public async Task<IActionResult> GetDownloadUrl(Guid id, CancellationToken ct)
    {
        var url = await _uploads.GetDownloadUrlAsync(id, ct);
        return url is null
            ? NotFound(ApiResponse<object>.Fail("NOT_FOUND", "File not found or has no stored document."))
            : Ok(ApiResponse<object>.Ok(new { url }));
    }







}
