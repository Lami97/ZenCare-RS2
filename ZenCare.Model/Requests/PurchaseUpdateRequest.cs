using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Model.Requests
{
    public class PurchaseUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(30)]
        public string PurchaseNumber { get; set; } = string.Empty;

        public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [MaxLength(150)]
        public string? StripePaymentIntentId { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
