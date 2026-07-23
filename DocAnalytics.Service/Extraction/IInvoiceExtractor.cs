namespace DocAnalytics.Service.Extraction;

/// <summary>Extracts structured invoice data from a PDF or JPEG document.</summary>
public interface IInvoiceExtractor
{
    /// <summary>Extracts invoice fields from the given file content.</summary>
    /// <param name="fileBytes">Raw file bytes (PDF or JPEG).</param>
    /// <param name="fileType">"pdf" or "jpeg" — controls the Bedrock content block type.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<InvoiceExtractionResult> ExtractAsync(
        byte[] fileBytes,
        string fileType,
        CancellationToken ct = default);
}
