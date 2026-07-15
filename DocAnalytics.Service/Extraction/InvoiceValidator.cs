namespace DocAnalytics.Service.Extraction;

public sealed class InvoiceValidator : IInvoiceValidator
{
    public ValidationOutcome Validate(InvoiceExtractionResult r)
    {
        bool hasCore = !string.IsNullOrWhiteSpace(r.Seller) && r.Total is not null;
        if (!hasCore)
            return new ValidationOutcome(0.0m, false, "ERR_UNREADABLE");

        var sumLines = r.LineItems.Sum(li => li.LineTotal ?? 0m);
        var subtotal = r.Subtotal ?? sumLines;         // fall back if omitted
        var discount = r.Discount ?? 0m;
        var tax = r.Tax ?? 0m;
        var shipping = r.Shipping ?? 0m;
        var total = r.Total ?? 0m;

        static decimal Tol(decimal v) => Math.Max(0.05m, Math.Abs(v) * 0.02m); // 2% or 5 cents

        // (a) line items should reconcile to the SUBTOTAL (not the grand total)
        bool subtotalOk = r.LineItems.Count > 0 && Math.Abs(sumLines - subtotal) <= Tol(subtotal);

        // (b) grand total should reconcile: subtotal − discount + tax + shipping ≈ total
        var computedTotal = subtotal - discount + tax + shipping;
        bool totalOk = total > 0 && Math.Abs(computedTotal - total) <= Tol(total);

        decimal confidence = (subtotalOk && totalOk) ? 0.98m
                           : (subtotalOk || totalOk) ? 0.80m
                           : 0.60m;

        bool lowConf = confidence < 0.70m;
        return new ValidationOutcome(confidence, !lowConf, lowConf ? "ERR_BEDROCK_LOWCONF" : null);
    }
}
