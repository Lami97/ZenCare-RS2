namespace ZenCare.Model.SearchObjects;

public class PagedSearchObject : BaseSearchObject
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
