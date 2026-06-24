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

    // GET /api/v1/batches/{id} — drill into ONE batch
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBatchById(Guid id, CancellationToken ct)
    {
        // 1. ask the service (tenant filter is auto-applied inside)
        var batch = await _batchService.GetBatchByIdAsync(id, ct);

        // 2. null = not found → 404 in the standard envelope
        if (batch is null)
            return NotFound(ApiResponse<BatchDetailDto>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        // 3. found → 200 single-resource envelope ({ data, error }, no meta)
        return Ok(ApiResponse<BatchDetailDto>.Ok(batch));
    }

    // GET /api/v1/batches/{id}/files — list the files in a batch (paged)
    [HttpGet("{id:guid}/files")]
    public async Task<IActionResult> GetBatchFiles(
        Guid id, [FromQuery] BatchFilesQuery query, CancellationToken ct)
    {
        // 1. delegate to the service (tenant filter auto-applied)
        var result = await _batchService.GetBatchFilesAsync(id, query, ct);

        // 2. null = batch not found → 404
        if (result is null)
            return NotFound(ApiResponse<List<BatchFileDto>>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        // 3. build the paging meta
        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        // 4. found → 200 list envelope ({ data, meta, error })
        return Ok(ApiResponse<List<BatchFileDto>>.OkList(result.Items, meta));
    }


}
