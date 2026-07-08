namespace DocAnalytics.Service.Extraction;

public sealed record ValidationOutcome(decimal Confidence, bool IsValid, string? ErrorCode);

public interface IInvoiceValidator
{
    ValidationOutcome Validate(InvoiceExtractionResult result);
}
