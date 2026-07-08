namespace DocAnalytics.Service.Extraction;

public sealed record InvoiceExtractionResult(
    string? InvoiceNumber,
    string? InvoiceDate,
    string? Seller,
    string? Client,
    decimal? Total,
    List<ExtractedLineItem> LineItems);

public sealed record ExtractedLineItem(
    int LineNumber,
    string Description,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? LineTotal);
