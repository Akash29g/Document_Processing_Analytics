namespace DocAnalytics.Service.Extraction;

public interface IInvoiceExtractor
{
    Task<InvoiceExtractionResult> ExtractAsync(byte[] pdfBytes, CancellationToken ct = default);
}
