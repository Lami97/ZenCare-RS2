using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class PurchaseItemInsertRequest
    {
        [Required]
        public int PurchaseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
}
