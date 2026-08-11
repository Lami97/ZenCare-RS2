namespace ZenCare.Model.SearchObjects
{
    public class ServiceSearchObject : PagedSearchObject
    {
        public string? Name { get; set; }

        public int? ServiceCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}
