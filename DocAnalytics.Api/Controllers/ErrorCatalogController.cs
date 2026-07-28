using DocAnalytics.Api.Common;
using DocAnalytics.Service.ErrorCatalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Global error catalog management.
/// GET is open to all authenticated users; POST and PUT require Admin role.
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]
[Route("api/v1/error-catalog")]
public sealed class ErrorCatalogController : ControllerBase
{
    private readonly IErrorCatalogService _service;

    public ErrorCatalogController(IErrorCatalogService service) => _service = service;

    /// <summary>Returns all error catalog entries ordered by code.</summary>
    // GET /api/v1/error-catalog
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<List<ErrorCatalogDto>>.Ok(items));
    }

    /// <summary>Creates a new error catalog entry. Admin only.</summary>
    // POST /api/v1/error-catalog
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateErrorCatalogDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.ErrorCode) || string.IsNullOrWhiteSpace(dto.Description))
            return BadRequest(ApiResponse<ErrorCatalogDto>.Fail(
                "VALIDATION", "ErrorCode and Description are required."));

        var result = await _service.CreateAsync(dto, ct);

        if (result is null)
            return Conflict(ApiResponse<ErrorCatalogDto>.Fail(
                "DUPLICATE_CODE", $"Error code '{dto.ErrorCode.Trim().ToUpperInvariant()}' already exists."));

        return Ok(ApiResponse<ErrorCatalogDto>.Ok(result));
    }

    /// <summary>Updates description and remediation of an existing entry. Admin only.</summary>
    // PUT /api/v1/error-catalog/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateErrorCatalogDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            return BadRequest(ApiResponse<ErrorCatalogDto>.Fail(
                "VALIDATION", "Description is required."));

        var result = await _service.UpdateAsync(id, dto, ct);

        if (result is null)
            return NotFound(ApiResponse<ErrorCatalogDto>.Fail(
                "NOT_FOUND", "Error catalog entry not found."));

        return Ok(ApiResponse<ErrorCatalogDto>.Ok(result));
    }
}
