namespace DocAnalytics.Service.Common;

/// <summary>A single page of results plus paging metadata.</summary>
/// <typeparam name="T">The row type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>The rows for this page.</summary>
    public List<T> Items { get; init; } = new();   // the rows for THIS page
    /// <summary>Total rows across all pages.</summary>
    public int TotalCount { get; init; }            // total rows across ALL pages
    /// <summary>The 1-based page number this result represents.</summary>
    public int Page { get; init; }                  // which page this is
    /// <summary>The number of rows per page.</summary>
    public int PageSize { get; init; }              // rows per page

    /// <summary>The total number of pages (ceiling of TotalCount / PageSize).</summary>
    // computed: e.g. 95 items / 20 per page = 5 pages
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;
}
