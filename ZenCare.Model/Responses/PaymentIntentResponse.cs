using ZenCare.Model.Enums;

namespace ZenCare.Model.Responses
{
    public class PaymentIntentResponse
    {
        public int PurchaseId { get; set; }

        public int PaymentId { get; set; }

        public string StripePaymentIntentId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string PublishableKey { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;

        public PaymentStatus Status { get; set; }
    }
}
