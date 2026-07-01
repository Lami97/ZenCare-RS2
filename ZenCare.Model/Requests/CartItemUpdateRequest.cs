using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class CartItemUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
