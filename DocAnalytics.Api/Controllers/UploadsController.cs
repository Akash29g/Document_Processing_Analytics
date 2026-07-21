using DocAnalytics.Api.Common;
using DocAnalytics.Service.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// File upload endpoints: presigned upload URLs, upload completion, batch create/shrink/delete, and download URLs.
/// </summary>
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[ApiController]
[Route("api/v1/files")]
public sealed class UploadsController : ControllerBase
{
    private readonly IUploadService _uploads;

    /// <summary>Creates a new <see cref="UploadsController"/>.</summary>
    /// <param name="uploads">Upload service.</param>
    public UploadsController(IUploadService uploads) => _uploads = uploads;

    /// <summary>Issues a short-lived presigned S3 URL for uploading a single file.</summary>
    /// <param name="req">File name, size, and duplicate-handling choice.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The presigned upload URL and file id.</returns>
    /// <response code="200">Upload URL issued.</response>
    /// <response code="409">A duplicate file was detected.</response>
    /// <response code="400">Upload rejected (e.g. bad type or oversize).</response>
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

    /// <summary>Marks an uploaded file as complete and enqueues it for extraction.</summary>
    /// <param name="id">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">File queued for processing.</response>
    /// <response code="404">File not found.</response>
    // POST /api/v1/files/{id}/complete
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.CompleteAsync(id, ct);
        return ok
            ? Ok(ApiResponse<object>.Ok(new { queued = true }))
            : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "File not found."));
    }

    /// <summary>Creates a new upload batch (transaction) for a planned set of files.</summary>
    /// <param name="req">The batch definition (e.g. file count).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created batch, or a rejection error.</returns>
    /// <response code="200">Batch created.</response>
    /// <response code="400">Batch rejected (invalid request).</response>
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

    /// <summary>Shrinks a batch's expected file count when a planned file is skipped.</summary>
    /// <param name="id">The batch id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">Batch shrunk.</response>
    /// <response code="404">Batch not found.</response>
    // POST /api/v1/files/batches/{id}/shrink — a planned file was skipped
    [HttpPost("batches/{id:guid}/shrink")]
    public async Task<IActionResult> ShrinkBatch(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.ShrinkBatchAsync(id, ct);
        return ok ? Ok(ApiResponse<object>.Ok(new { shrunk = true }))
                  : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Batch not found."));
    }

    /// <summary>Deletes a batch and its files (Admin only), including the stored S3 objects.</summary>
    /// <param name="id">The batch id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success, or a not-found envelope.</returns>
    /// <response code="200">Batch deleted.</response>
    /// <response code="404">Batch not found.</response>
    // DELETE /api/v1/files/batches/{id} — admin only
    [HttpDelete("batches/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBatch(Guid id, CancellationToken ct)
    {
        var ok = await _uploads.DeleteBatchAsync(id, ct);
        return ok ? Ok(ApiResponse<object>.Ok(new { deleted = true }))
                  : NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Batch not found."));
    }

    /// <summary>Returns a short-lived presigned S3 GET URL to download a file's stored document.</summary>
    /// <param name="id">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The presigned download URL, or a not-found envelope.</returns>
    /// <response code="200">Download URL issued.</response>
    /// <response code="404">File not found or has no stored document.</response>
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
