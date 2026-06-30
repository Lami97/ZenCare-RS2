using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class AppointmentSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public int? EmployeeId { get; set; }

        public int? WellnessServiceId { get; set; }

        public AppointmentStatus? Status { get; set; }

        public DateTime? AppointmentDate { get; set; }
    }
}
