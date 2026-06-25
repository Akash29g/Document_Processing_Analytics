using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>  (adjust if your namespace differs)
using DocAnalytics.Service.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IFileDetailsService _service;
    public FilesController(IFileDetailsService service) => _service = service;

    // GET /api/v1/files/{id}/details
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var result = await _service.GetFileDetailsAsync(id, ct);
        return result.Status switch
        {
            LookupStatus.Found => Ok(ApiResponse<FileDetailDto>.Ok(result.Value!)),
            LookupStatus.Forbidden => StatusCode(403, ApiResponse<FileDetailDto>.Fail(
                                          "FORBIDDEN", "You do not have access to this file.")),
            _ => NotFound(ApiResponse<FileDetailDto>.Fail(
                                          "NOT_FOUND", "File not found."))
        };
    }

    // GET /api/v1/files/{id}/logs  → downloads a .txt
    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs(Guid id, CancellationToken ct)
    {
        var result = await _service.GetFileLogsAsync(id, ct);
        return result.Status switch
        {
            LookupStatus.Found => File(Encoding.UTF8.GetBytes(result.Value!.Content),
                                           "text/plain", result.Value.FileName),
            LookupStatus.Forbidden => StatusCode(403, ApiResponse<object>.Fail(
                                          "FORBIDDEN", "You do not have access to this file.")),
            _ => NotFound(ApiResponse<object>.Fail(
                                          "NOT_FOUND", "File not found."))
        };
    }
}
