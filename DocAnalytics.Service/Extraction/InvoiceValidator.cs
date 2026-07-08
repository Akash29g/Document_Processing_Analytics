namespace DocAnalytics.Service.Extraction;

public sealed class InvoiceValidator : IInvoiceValidator
{
    public ValidationOutcome Validate(InvoiceExtractionResult r)
    {
        // hard fail: nothing usable came back
        bool hasCore = !string.IsNullOrWhiteSpace(r.Seller) && r.Total is not null;
        if (!hasCore)
            return new ValidationOutcome(0.0m, false, "ERR_UNREADABLE");

        decimal confidence = 0.60m;   // baseline: core fields present

        // do the line items add up to the total? (strong signal → high confidence)
        if (r.LineItems.Count > 0 && r.Total is > 0)
        {
            var sum = r.LineItems.Sum(li => li.LineTotal ?? 0m);
            var diff = Math.Abs(sum - r.Total.Value);
            var tolerance = r.Total.Value * 0.02m;   // 2%
            if (diff <= tolerance) confidence = 0.95m;
        }

        bool lowConf = confidence < 0.70m;
        return new ValidationOutcome(
            confidence,
            IsValid: !lowConf,
            ErrorCode: lowConf ? "ERR_BEDROCK_LOWCONF" : null);
    }
}
