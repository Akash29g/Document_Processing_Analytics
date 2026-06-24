namespace DocAnalytics.Service.Invoices;

// The whole response: one file's invoice line items + computed totals.
public sealed class InvoiceDetailDto
{
    public Guid FileId { get; set; }
    public int LineItemCount { get; set; }
    public decimal GrandTotal { get; set; }              // a total is always a number
    public List<InvoiceLineItemDto> Items { get; set; } = new();
}

// One row on the invoice: the line item + its category (joined from the global catalog).
public sealed class InvoiceLineItemDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public string Description { get; set; } = null!;
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
    public decimal? Confidence { get; set; }
    public bool IsValid { get; set; }
    public string? CategoryCode { get; set; }    // null when the line has no category (LEFT join)
    public string? CategoryName { get; set; }    // null when the line has no category (LEFT join)
}
