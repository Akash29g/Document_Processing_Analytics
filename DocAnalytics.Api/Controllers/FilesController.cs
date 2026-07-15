using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>  (adjust if your namespace differs)
using DocAnalytics.Service.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{

    private readonly IFileDetailsService _service;
    public FilesController(IFileDetailsService service) => _service = service;

    // GET /api/v1/files/{id}/details
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetFileDetailsAsync(id, ct);
        if (dto is null)
            return NotFound(ApiResponse<FileDetailDto>.Fail("NOT_FOUND", "File not found."));
        return Ok(ApiResponse<FileDetailDto>.Ok(dto));
    }

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
