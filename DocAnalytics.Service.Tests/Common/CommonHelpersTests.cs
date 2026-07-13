using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Tests.Common;

public class OneOfAttributeTests
{
    private readonly OneOfAttribute _attr = new("asc", "desc");

    [Fact] public void Valid_when_value_in_list() => Assert.True(_attr.IsValid("asc"));
    [Fact] public void Valid_case_insensitive() => Assert.True(_attr.IsValid("DESC"));
    [Fact] public void Valid_when_null_or_blank() { Assert.True(_attr.IsValid(null)); Assert.True(_attr.IsValid("  ")); }
    [Fact] public void Invalid_when_not_in_list() => Assert.False(_attr.IsValid("sideways"));
    [Fact] public void FormatErrorMessage_lists_allowed_values() => Assert.Contains("asc", _attr.FormatErrorMessage("SortDir"));
}

public class DateTimeExtensionsTests
{
    [Fact]
    public void AsUtc_keeps_utc()
        => Assert.Equal(DateTimeKind.Utc, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc).AsUtc().Kind);
    [Fact]
    public void AsUtc_converts_local()
        => Assert.Equal(DateTimeKind.Utc, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local).AsUtc().Kind);
    [Fact]
    public void AsUtc_specifies_unspecified_as_utc()
        => Assert.Equal(DateTimeKind.Utc, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AsUtc().Kind);
}

public class PagedResultTests
{
    [Fact]
    public void TotalPages_rounds_up()
        => Assert.Equal(5, new PagedResult<int> { TotalCount = 95, PageSize = 20 }.TotalPages);
    [Fact]
    public void TotalPages_zero_when_pagesize_zero()
        => Assert.Equal(0, new PagedResult<int> { TotalCount = 10, PageSize = 0 }.TotalPages);
}
