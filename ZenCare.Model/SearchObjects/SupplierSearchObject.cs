namespace ZenCare.Model.SearchObjects
{
    public class SupplierSearchObject : PagedSearchObject
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
