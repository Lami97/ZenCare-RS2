using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class ReviewSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public int? AppointmentId { get; set; }

        public int? ProductId { get; set; }

        public int? Rating { get; set; }

        public ReviewStatus? Status { get; set; }
    }
}
