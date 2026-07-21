using DocAnalytics.Api.Common;
using DocAnalytics.Service.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

/// <summary>
/// Invoice line-item endpoint: returns the extracted invoice detail for a file.
/// </summary>
[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/files")]
public sealed class InvoiceLineItemsController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    /// <summary>Creates a new <see cref="InvoiceLineItemsController"/>.</summary>
    /// <param name="invoiceService">Invoice extraction/query service.</param>
    public InvoiceLineItemsController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    /// <summary>Returns the invoice header and line items extracted from a file.</summary>
    /// <param name="id">The file id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The invoice detail, or a not-found envelope.</returns>
    /// <response code="200">Invoice detail returned.</response>
    /// <response code="404">File not found for this tenant/site.</response>
    // GET /api/v1/files/{id}/line-items
    [HttpGet("{id:guid}/line-items")]
    public async Task<IActionResult> GetLineItems(Guid id, CancellationToken ct)
    {
        var invoice = await _invoiceService.GetInvoiceForFileAsync(id, ct);

        if (invoice is null)
            return NotFound(ApiResponse<InvoiceDetailDto>.Fail(
                "not_found", $"File '{id}' was not found."));

        return Ok(ApiResponse<InvoiceDetailDto>.Ok(invoice));
    }
}
