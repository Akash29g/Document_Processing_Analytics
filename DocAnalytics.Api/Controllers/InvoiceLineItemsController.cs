using DocAnalytics.Api.Common;
using DocAnalytics.Service.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize(Policy = "DataAccess")]   // ← was [Authorize]
[Route("api/v1/files")]
public sealed class InvoiceLineItemsController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    public InvoiceLineItemsController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

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
