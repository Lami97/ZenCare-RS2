namespace ZenCare.Model.SearchObjects
{
    public class ProductTypeSearchObject : PagedSearchObject
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
