using ZenCare.Model.Enums;

namespace ZenCare.Model.Responses
{
    public class PaymentRefundResponse
    {
        public int PurchaseId { get; set; }

        public int PaymentId { get; set; }

        public PurchaseStatus PurchaseStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? RefundedAt { get; set; }
    }
}
