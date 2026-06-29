namespace ZenCare.Model.SearchObjects
{
    public class ProductSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }

        public int? ProductCategoryId { get; set; }

        public int? ProductTypeId { get; set; }

        public bool? IsActive { get; set; }
    }
}
