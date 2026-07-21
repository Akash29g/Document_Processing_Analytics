using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta  (drop if global usings)
using DocAnalytics.Service.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Error analysis endpoints: filtered/paginated error list and CSV export (FR-3).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/errors")]
[EnableRateLimiting("reads")]        // ← NEW: per-user cap on all read endpoints here
public sealed class ErrorsController : ControllerBase
{
    private readonly IErrorService _errors;

    /// <summary>Creates a new <see cref="ErrorsController"/>.</summary>
    /// <param name="errors">Error query/export service.</param>
    public ErrorsController(IErrorService errors) => _errors = errors;

    /// <summary>Returns a filtered, paginated list of processing errors for the selected tenant/site (FR-3.4).</summary>
    /// <param name="query">Filter, sort, and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of errors with metadata.</returns>
    /// <response code="200">Errors returned.</response>
    // GET /api/v1/errors — filtered + paginated error list (FR-3.4)
    [HttpGet]
    public async Task<IActionResult> GetErrors([FromQuery] ErrorListQuery query, CancellationToken ct)
    {
        var result = await _errors.GetErrorsAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<ErrorListItemDto>>.OkList(result.Items, meta));
    }

    /// <summary>Exports the filtered error list as a UTF-8 (BOM) CSV attachment (FR-3.5).</summary>
    /// <param name="query">Same filters as <see cref="GetErrors"/>.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file download.</returns>
    /// <response code="200">CSV file returned.</response>
    /// <response code="429">Export rate limit exceeded.</response>
    // GET /api/v1/errors/export — CSV of the filtered list (FR-3.5)
    [HttpGet("export")]
    [EnableRateLimiting("export")]   // ← NEW: tight limit, overrides the class-level "reads"
    public async Task<IActionResult> ExportErrors([FromQuery] ErrorListQuery query, CancellationToken ct)
    {
        var rows = await _errors.GetErrorsForExportAsync(query, ct);
        var csv = ErrorCsvWriter.Write(rows);

        // UTF-8 BOM so Excel renders accents/symbols correctly
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv))
            .ToArray();

        var fileName = $"errors_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

        // File(...) with a download name sets Content-Disposition: attachment automatically
        return File(bytes, "text/csv", fileName);
    }
}
