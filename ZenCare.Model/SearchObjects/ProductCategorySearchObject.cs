namespace ZenCare.Model.SearchObjects
{
    public class ProductCategorySearchObject : PagedSearchObject
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
