using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class PurchaseCheckoutRequest
    {
        [Required]
        public List<PurchaseCheckoutItemRequest> Items { get; set; } = new List<PurchaseCheckoutItemRequest>();
    }
}
