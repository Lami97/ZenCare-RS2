namespace ZenCare.Model.SearchObjects
{
    public class CartItemSearchObject : PagedSearchObject
    {
        public int? CartId { get; set; }

        public int? ProductId { get; set; }
    }
}
