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

        // 3. Compute totals over ALL lines (null-safe sum).
        var grandTotal = items.Sum(i => i.LineTotal ?? 0m);

        // 4. Assemble the detail response.
        return new InvoiceDetailDto
        {
            FileId = fileId,
            LineItemCount = items.Count,
            GrandTotal = grandTotal,
            Items = items
        };
    }
}
