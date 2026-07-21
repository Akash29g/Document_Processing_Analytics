namespace DocAnalytics.Service.Extraction;

/// <summary>The outcome of validating an extraction: derived confidence, validity, and an optional error code.</summary>
/// <param name="Confidence">Confidence score (0–1) reflecting how cleanly the values reconciled.</param>
/// <param name="IsValid">Whether the extraction passed validation.</param>
/// <param name="ErrorCode">The error code when invalid; otherwise <c>null</c>.</param>
public sealed record ValidationOutcome(decimal Confidence, bool IsValid, string? ErrorCode);

/// <summary>Validates an invoice extraction result and derives a confidence score.</summary>
public interface IInvoiceValidator
{
    /// <summary>Validates an extraction result (e.g. totals reconcile, required fields present).</summary>
    /// <param name="result">The extraction result to validate.</param>
    /// <returns>The validation outcome.</returns>
    ValidationOutcome Validate(InvoiceExtractionResult result);
}
