namespace ZenCare.Model.SearchObjects
{
    public class ServiceSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }

        public int? ServiceCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}
