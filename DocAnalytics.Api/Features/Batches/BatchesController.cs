using DocAnalytics.Api.Common;
using DocAnalytics.Service.Abstractions;
using DocAnalytics.Service.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Features.Batches;

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
}
