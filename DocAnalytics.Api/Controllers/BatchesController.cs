using DocAnalytics.Api.Common;
using DocAnalytics.Service.Batches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Batch Explorer endpoints: paginated batch list, source-system filters,
/// batch detail, and the files within a batch (FR-2).
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/batches")]
public sealed class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    /// <summary>Creates a new <see cref="BatchesController"/>.</summary>
    /// <param name="batchService">Batch query service (injected via its interface).</param>
    // the service is injected (depends on the INTERFACE, not the class)
    public BatchesController(IBatchService batchService) => _batchService = batchService;

    /// <summary>Returns a filtered, paginated list of batches for the selected tenant/site (FR-2.1–2.3).</summary>
    /// <param name="query">Filter, search, sort, and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of batches with pagination metadata.</returns>
    /// <response code="200">Batch list returned.</response>
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

    /// <summary>Returns the distinct source systems used by batches, for the FilterBar dropdown.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of distinct source-system names.</returns>
    /// <response code="200">Source list returned.</response>
    // GET /api/v1/batches/sources — distinct source systems for the FilterBar dropdown
    [HttpGet("sources")]
    public async Task<IActionResult> GetSources(CancellationToken ct)
    {
        var sources = await _batchService.GetSourcesAsync(ct);
        return Ok(ApiResponse<List<string>>.Ok(sources));
    }

    /// <summary>Returns the detail summary for a single batch (FR-2.4).</summary>
    /// <param name="id">The batch (transaction) id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The batch detail, or a not-found envelope.</returns>
    /// <response code="200">Batch found.</response>
    /// <response code="404">Batch does not exist for this tenant/site.</response>
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

    /// <summary>Returns a paginated list of the files contained in a batch (FR-2.4).</summary>
    /// <param name="id">The batch (transaction) id.</param>
    /// <param name="query">Pagination and sort parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of the batch's files, or a not-found envelope.</returns>
    /// <response code="200">Files returned.</response>
    /// <response code="404">Batch does not exist for this tenant/site.</response>
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
