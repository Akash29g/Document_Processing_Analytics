namespace DocAnalytics.Service.Extraction;

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


public sealed record ExtractedLineItem(
    int LineNumber,
    string Description,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? LineTotal,
    string? Category);
