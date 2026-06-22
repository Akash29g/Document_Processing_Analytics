namespace DocAnalytics.Service.Common;

public sealed class PagedResult<T>
{
    public List<T> Items { get; init; } = new();   // the rows for THIS page
    public int TotalCount { get; init; }            // total rows across ALL pages
    public int Page { get; init; }                  // which page this is
    public int PageSize { get; init; }              // rows per page

    // computed: e.g. 95 items / 20 per page = 5 pages
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;
}
