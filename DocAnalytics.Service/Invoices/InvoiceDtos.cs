namespace DocAnalytics.Service.Invoices;

/// <summary>Full invoice response for one file: header, line items, and computed totals.</summary>
// The whole response: one file's invoice line items + computed totals.
public sealed class InvoiceDetailDto
{
    /// <summary>The file id this invoice belongs to.</summary>
    public Guid FileId { get; set; }
    /// <summary>The invoice header (1:1 with the file), if present.</summary>
    public InvoiceHeaderDto? Header { get; set; }
    /// <summary>The number of line items.</summary>
    public int LineItemCount { get; set; }
    /// <summary>The grand total (header total when available, otherwise the line sum).</summary>
    public decimal GrandTotal { get; set; }              // a total is always a number
    /// <summary>The invoice line items.</summary>
    public List<InvoiceLineItemDto> Items { get; set; } = new();
}

/// <summary>One invoice line item with its (optionally joined) category.</summary>
// One row on the invoice: the line item + its category (joined from the global catalog).
public sealed class InvoiceLineItemDto
{
    /// <summary>The line item id.</summary>
    public Guid Id { get; set; }
    /// <summary>1-based line number.</summary>
    public int LineNumber { get; set; }
    /// <summary>Item description.</summary>
    public string Description { get; set; } = null!;
    /// <summary>Quantity, if present.</summary>
    public decimal? Quantity { get; set; }
    /// <summary>Unit price, if present.</summary>
    public decimal? UnitPrice { get; set; }
    /// <summary>Line total, if present.</summary>
    public decimal? LineTotal { get; set; }
    /// <summary>Extraction confidence score, if present.</summary>
    public decimal? Confidence { get; set; }
    /// <summary>Whether the line passed validation.</summary>
    public bool IsValid { get; set; }
    /// <summary>Category code (null when the line has no category — LEFT join).</summary>
    public string? CategoryCode { get; set; }    // null when the line has no category (LEFT join)
    /// <summary>Category name (null when the line has no category — LEFT join).</summary>
    public string? CategoryName { get; set; }    // null when the line has no category (LEFT join)
}

/// <summary>Invoice header fields (1:1 with a file).</summary>
public sealed class InvoiceHeaderDto
{
    /// <summary>Invoice number, if present.</summary>
    public string? InvoiceNumber { get; set; }
    /// <summary>Invoice date, if present.</summary>
    public string? InvoiceDate { get; set; }
    /// <summary>The business issuing the invoice.</summary>
    public string? Seller { get; set; }
    /// <summary>The recipient being billed.</summary>
    public string? Buyer { get; set; }
    /// <summary>ISO currency code.</summary>
    public string? Currency { get; set; }
    /// <summary>Sum before discount/tax/shipping.</summary>
    public decimal? Subtotal { get; set; }
    /// <summary>Discount amount.</summary>
    public decimal? Discount { get; set; }
    /// <summary>Tax amount.</summary>
    public decimal? Tax { get; set; }
    /// <summary>Shipping/freight amount.</summary>
    public decimal? Shipping { get; set; }
    /// <summary>Final payable grand total.</summary>
    public decimal? Total { get; set; }
}
