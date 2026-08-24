using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class TimeSlotInsertRequest
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int WellnessServiceId { get; set; }

    [Required]
    public DateTime SlotDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; } = true;
}
