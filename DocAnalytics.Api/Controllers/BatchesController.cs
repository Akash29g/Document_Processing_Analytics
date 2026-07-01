using DocAnalytics.Api.Common;
using DocAnalytics.Service.Batches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/batches")]
public sealed class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    // the service is injected (depends on the INTERFACE, not the class)
    public BatchesController(IBatchService batchService) => _batchService = batchService;

    [HttpGet]
    public async Task<IActionResult> GetBatches(
        [FromQuery] BatchListQuery query, CancellationToken ct)
    {
        // 1. delegate the real work to the service
        var result = await _batchService.GetBatchesAsync(query, ct);

        // 2. build the paging meta
        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        // 3. wrap in the standard envelope and return
        return Ok(ApiResponse<List<BatchListItemDto>>.OkList(result.Items, meta));
    }

    // GET /api/v1/batches/sources — distinct source systems for the FilterBar dropdown
    [HttpGet("sources")]
    public async Task<IActionResult> GetSources(CancellationToken ct)
    {
        var sources = await _batchService.GetSourcesAsync(ct);
        return Ok(ApiResponse<List<string>>.Ok(sources));
    }

    // GET /api/v1/batches/{id} — one batch's detail
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBatchById(Guid id, CancellationToken ct)
    {
        var batch = await _batchService.GetBatchByIdAsync(id, ct);

        if (batch is null)
            return NotFound(ApiResponse<BatchDetailDto>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        return Ok(ApiResponse<BatchDetailDto>.Ok(batch));
    }

    // GET /api/v1/batches/{id}/files — paged list of a batch's files
    [HttpGet("{id:guid}/files")]
    public async Task<IActionResult> GetBatchFiles(
        Guid id, [FromQuery] BatchFilesQuery query, CancellationToken ct)
    {
        var result = await _batchService.GetBatchFilesAsync(id, query, ct);

        if (result is null)
            return NotFound(ApiResponse<List<BatchFileDto>>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<BatchFileDto>>.OkList(result.Items, meta));
    }

}
