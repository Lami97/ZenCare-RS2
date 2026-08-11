namespace ZenCare.Model.SearchObjects
{
    public class FAQCategorySearchObject : PagedSearchObject
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
