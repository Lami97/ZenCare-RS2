namespace ZenCare.Model.SearchObjects;

public class BaseSearchObject
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool? IncludeTotalCount { get; set; }
}
