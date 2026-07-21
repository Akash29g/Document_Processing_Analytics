namespace DocAnalytics.Service.Extraction;

/// <summary>Extracts structured invoice data (header + line items) from a PDF's bytes.</summary>
public interface IInvoiceExtractor
{
    /// <summary>Extracts invoice fields from the given PDF content.</summary>
    /// <param name="pdfBytes">The raw PDF file bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The extraction result (fields, line items, and confidence).</returns>
    Task<InvoiceExtractionResult> ExtractAsync(byte[] pdfBytes, CancellationToken ct = default);
}
