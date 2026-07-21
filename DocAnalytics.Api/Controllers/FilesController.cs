using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>  (adjust if your namespace differs)
using DocAnalytics.Service.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// File detail endpoints: step-history details and downloadable step logs (FR-2.5, FR-3.3).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{

    private readonly IFileDetailsService _service;

    /// <summary>Creates a new <see cref="FilesController"/>.</summary>
    /// <param name="service">File details service.</param>
    public FilesController(IFileDetailsService service) => _service = service;

    /// <summary>Returns a file's info plus its full step history (FR-2.5 &amp; FR-3.3).</summary>
    /// <param name="id">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The file details, or a not-found envelope.</returns>
    /// <response code="200">File details returned.</response>
    /// <response code="404">File not found.</response>
    // GET /api/v1/files/{id}/details
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetFileDetailsAsync(id, ct);
        if (dto is null)
            return NotFound(ApiResponse<FileDetailDto>.Fail("NOT_FOUND", "File not found."));
        return Ok(ApiResponse<FileDetailDto>.Ok(dto));
    }

    /// <summary>Downloads a plain-text log built from a file's step history.</summary>
    /// <param name="id">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <c>text/plain</c> log file, or a not-found envelope.</returns>
    /// <response code="200">Log file returned.</response>
    /// <response code="404">File not found.</response>
    // GET /api/v1/files/{id}/logs  → downloads a .txt
    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs(Guid id, CancellationToken ct)
    {
        var log = await _service.GetFileLogsAsync(id, ct);
        if (log is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "File not found."));
        return File(Encoding.UTF8.GetBytes(log.Content), "text/plain", log.FileName);
    }


}
