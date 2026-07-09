using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Invoices;

public sealed class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;
    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<InvoiceDetailDto?> GetInvoiceForFileAsync(Guid fileId, CancellationToken ct = default)
    {
        // 1. Does this file exist for THIS tenant/site? (Files is tenant-scoped → auto-filtered)
        var fileExists = await _db.Files
            .AsNoTracking()
            .AnyAsync(f => f.Id == fileId, ct);

        if (!fileExists)
            return null;                       // → controller turns this into 404

        // 2. Pull this file's line items, LEFT-joined out to the global category catalog.
        var items = await _db.InvoiceLineItems
            .AsNoTracking()
            .Where(li => li.FileId == fileId)  // tenant_id + site_id auto-added by the global filter
            .OrderBy(li => li.LineNumber)
            .Select(li => new InvoiceLineItemDto
            {
                Id = li.Id,
                LineNumber = li.LineNumber,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                LineTotal = li.LineTotal,
                Confidence = li.Confidence,
                IsValid = li.IsValid,
                CategoryCode = li.ItemCategory != null ? li.ItemCategory.CategoryCode : null,
                CategoryName = li.ItemCategory != null ? li.ItemCategory.CategoryName : null
            })
            .ToListAsync(ct);

        // 3. NEW — pull the invoice header (1:1 with the file)
        var header = await _db.InvoiceHeaders.AsNoTracking()
            .Where(h => h.FileId == fileId)
            .Select(h => new InvoiceHeaderDto
            {
                InvoiceNumber = h.InvoiceNumber,
                InvoiceDate = h.InvoiceDate,
                Seller = h.Seller,
                Buyer = h.Buyer,
                Currency = h.Currency,
                Subtotal = h.Subtotal,
                Discount = h.Discount,
                Tax = h.Tax,
                Shipping = h.Shipping,
                Total = h.Total,
            })
            .FirstOrDefaultAsync(ct);

        var lineSum = items.Sum(i => i.LineTotal ?? 0m);

        // 4. NEW — assemble with header; grand total = the REAL total (incl. shipping)
        // 4. NEW — assemble with header; grand total = the REAL total (incl. shipping)
        return new InvoiceDetailDto
        {
            FileId = fileId,
            Header = header,
            Items = items,
            LineItemCount = items.Count,
            GrandTotal = header?.Total ?? lineSum,
        };

    }
}
