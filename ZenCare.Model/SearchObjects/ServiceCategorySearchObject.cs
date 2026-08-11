namespace ZenCare.Model.SearchObjects
{
    public class ServiceCategorySearchObject : PagedSearchObject
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
