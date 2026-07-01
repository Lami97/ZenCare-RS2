using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class PurchaseSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public string? PurchaseNumber { get; set; }

        public PurchaseStatus? Status { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
    }
}
