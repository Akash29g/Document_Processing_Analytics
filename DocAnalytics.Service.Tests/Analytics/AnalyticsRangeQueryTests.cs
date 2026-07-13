using System.ComponentModel.DataAnnotations;
using DocAnalytics.Service.Analytics;

namespace DocAnalytics.Service.Tests.Analytics;

public class AnalyticsRangeQueryTests
{
    private static List<ValidationResult> Validate(AnalyticsRangeQuery q) =>
        q.Validate(new ValidationContext(q)).ToList();

    [Fact]
    public void Invalid_when_from_after_to()
    {
        var q = new AnalyticsRangeQuery { From = new DateTime(2026, 6, 1), To = new DateTime(2026, 1, 1) };
        Assert.Single(Validate(q));
    }

    [Fact]
    public void Valid_when_from_before_to()
    {
        var q = new AnalyticsRangeQuery { From = new DateTime(2026, 1, 1), To = new DateTime(2026, 6, 1) };
        Assert.Empty(Validate(q));
    }

    [Fact]
    public void Valid_when_bounds_missing()
    {
        Assert.Empty(Validate(new AnalyticsRangeQuery()));
    }
}
