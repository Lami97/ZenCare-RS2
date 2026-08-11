namespace ZenCare.Model.SearchObjects
{
    public class FAQSearchObject : PagedSearchObject
    {
        public string? Question { get; set; }

        public int? FAQCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}
