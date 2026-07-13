using System.ComponentModel.DataAnnotations;
using DocAnalytics.Service.Batches;

namespace DocAnalytics.Service.Tests.Batches;

public class BatchListQueryTests
{
    private static List<ValidationResult> Validate(BatchListQuery q) => q.Validate(new ValidationContext(q)).ToList();

    [Fact]
    public void Invalid_when_from_after_to()
        => Assert.Single(Validate(new BatchListQuery { From = new DateTime(2026, 6, 1), To = new DateTime(2026, 1, 1) }));
    [Fact]
    public void Valid_when_from_before_to()
        => Assert.Empty(Validate(new BatchListQuery { From = new DateTime(2026, 1, 1), To = new DateTime(2026, 6, 1) }));
    [Fact]
    public void Valid_when_bounds_missing()
        => Assert.Empty(Validate(new BatchListQuery()));
}
