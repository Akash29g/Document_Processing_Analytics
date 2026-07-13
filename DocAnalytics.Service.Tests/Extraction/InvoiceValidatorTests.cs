using DocAnalytics.Service.Extraction;

namespace DocAnalytics.Service.Tests.Extraction;

public class InvoiceValidatorTests
{
    private readonly InvoiceValidator _sut = new();

    private static InvoiceExtractionResult Result(
        string? seller, decimal? total,
        decimal? subtotal = null, decimal? discount = null,
        decimal? tax = null, decimal? shipping = null,
        ExtractedLineItem[]? lines = null) =>
        new(
            InvoiceNumber: "INV-1",
            InvoiceDate: "2026-01-01",
            Seller: seller,
            Client: "Buyer",
            Currency: "USD",
            Subtotal: subtotal,
            Discount: discount,
            Tax: tax,
            Shipping: shipping,
            Total: total,
            LineItems: (lines ?? Array.Empty<ExtractedLineItem>()).ToList());

    private static ExtractedLineItem Line(decimal lineTotal) =>
        new(1, "Item", 1m, lineTotal, lineTotal, "Other");

    [Fact]
    public void Validate_returns_unreadable_when_seller_missing()
    {
        var o = _sut.Validate(Result(seller: null, total: 100m, lines: new[] { Line(100m) }));
        Assert.Equal("ERR_UNREADABLE", o.ErrorCode);
        Assert.False(o.IsValid);
        Assert.Equal(0.0m, o.Confidence);
    }

    [Fact]
    public void Validate_returns_unreadable_when_total_missing()
    {
        var o = _sut.Validate(Result(seller: "Acme", total: null, lines: new[] { Line(100m) }));
        Assert.Equal("ERR_UNREADABLE", o.ErrorCode);
        Assert.False(o.IsValid);
    }

    [Fact]
    public void Validate_high_confidence_when_subtotal_and_total_reconcile()
    {
        var o = _sut.Validate(Result(
            seller: "Acme", total: 100m, subtotal: 100m,
            lines: new[] { Line(60m), Line(40m) }));
        Assert.Equal(0.98m, o.Confidence);
        Assert.True(o.IsValid);
        Assert.Null(o.ErrorCode);
    }

    [Fact]
    public void Validate_medium_confidence_when_only_subtotal_reconciles()
    {
        // subtotal matches line sum, but grand total is way off
        var o = _sut.Validate(Result(
            seller: "Acme", total: 200m, subtotal: 100m,
            lines: new[] { Line(100m) }));
        Assert.Equal(0.80m, o.Confidence);
        Assert.True(o.IsValid);          // 0.80 >= 0.70
        Assert.Null(o.ErrorCode);
    }

    [Fact]
    public void Validate_low_confidence_when_nothing_reconciles()
    {
        // no line items + total mismatch → neither check passes
        var o = _sut.Validate(Result(seller: "Acme", total: 200m));
        Assert.Equal(0.60m, o.Confidence);
        Assert.False(o.IsValid);
        Assert.Equal("ERR_BEDROCK_LOWCONF", o.ErrorCode);
    }
}
