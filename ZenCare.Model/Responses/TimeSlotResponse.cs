namespace ZenCare.Model.Responses;

public class TimeSlotResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int WellnessServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public DateTime SlotDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsBooked { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
