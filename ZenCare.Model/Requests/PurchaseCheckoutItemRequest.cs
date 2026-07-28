using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class PurchaseCheckoutItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
