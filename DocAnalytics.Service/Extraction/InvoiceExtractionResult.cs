namespace DocAnalytics.Service.Extraction;

/// <summary>The structured result of extracting an invoice from a PDF (header fields + line items).</summary>
/// <param name="InvoiceNumber">Invoice number, if present.</param>
/// <param name="InvoiceDate">Invoice date (ISO yyyy-MM-dd), if present.</param>
/// <param name="Seller">The business issuing the invoice.</param>
/// <param name="Client">The recipient being billed.</param>
/// <param name="Currency">ISO currency code.</param>
/// <param name="Subtotal">Sum of line amounts before discount/tax/shipping.</param>
/// <param name="Discount">Discount amount.</param>
/// <param name="Tax">Tax amount.</param>
/// <param name="Shipping">Shipping/freight amount.</param>
/// <param name="Total">Final payable grand total.</param>
/// <param name="LineItems">The extracted line items.</param>
public sealed record InvoiceExtractionResult(
    string? InvoiceNumber,
    string? InvoiceDate,
    string? Seller,
    string? Client,
    string? Currency,
    decimal? Subtotal,
    decimal? Discount,
    decimal? Tax,
    decimal? Shipping,
    decimal? Total,
    List<ExtractedLineItem> LineItems);


/// <summary>A single extracted invoice line item.</summary>
/// <param name="LineNumber">1-based line number.</param>
/// <param name="Description">Item description.</param>
/// <param name="Quantity">Quantity, if present.</param>
/// <param name="UnitPrice">Unit price, if present.</param>
/// <param name="LineTotal">Line total, if present.</param>
/// <param name="Category">Item category, if classified.</param>
public sealed record ExtractedLineItem(
    int LineNumber,
    string Description,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? LineTotal,
    string? Category);
