using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta  (drop if global usings)
using DocAnalytics.Service.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/errors")]
[EnableRateLimiting("reads")]        // ← NEW: per-user cap on all read endpoints here
public sealed class ErrorsController : ControllerBase
{
    private readonly IErrorService _errors;
    public ErrorsController(IErrorService errors) => _errors = errors;

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
