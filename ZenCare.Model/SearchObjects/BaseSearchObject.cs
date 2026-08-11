namespace ZenCare.Model.SearchObjects;

public class BaseSearchObject
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool? IncludeTotalCount { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public static (int Page, int PageSize) NormalizePagination(BaseSearchObject? search)
    {
        var page = search?.Page ?? DefaultPage;
        var pageSize = search?.PageSize ?? DefaultPageSize;

        if (page <= 0)
        {
            page = DefaultPage;
        }

        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        return (page, pageSize);
    }
}
