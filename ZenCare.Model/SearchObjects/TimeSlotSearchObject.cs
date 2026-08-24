using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects;

public class TimeSlotSearchObject : PagedSearchObject
{
    public int? EmployeeId { get; set; }
    public int? WellnessServiceId { get; set; }
    public DateTime? SlotDate { get; set; }
    public TimeSlotStatus? Status { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsAvailable { get; set; }
}
