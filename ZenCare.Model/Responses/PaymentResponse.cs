using ZenCare.Model.Enums;

namespace ZenCare.Model.Responses
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? AppointmentId { get; set; }
        public int? PurchaseId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? StripeChargeId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
