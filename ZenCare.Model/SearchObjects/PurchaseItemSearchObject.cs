namespace ZenCare.Model.SearchObjects
{
    public class PurchaseItemSearchObject : PagedSearchObject
    {
        public int? PurchaseId { get; set; }

        public int? ProductId { get; set; }
    }
}
