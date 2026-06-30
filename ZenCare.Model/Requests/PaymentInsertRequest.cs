using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Model.Requests
{
    public class PaymentInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        public int? AppointmentId { get; set; }

        public int? PurchaseId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(150)]
        public string? StripePaymentIntentId { get; set; }

        [MaxLength(150)]
        public string? StripeChargeId { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
