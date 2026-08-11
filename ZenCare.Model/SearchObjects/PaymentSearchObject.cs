using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class PaymentSearchObject : PagedSearchObject
    {
        public int? UserId { get; set; }

        public int? AppointmentId { get; set; }

        public int? PurchaseId { get; set; }

        public PaymentStatus? Status { get; set; }
    }
}
