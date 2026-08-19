using ZenCare.Model.Enums;

namespace ZenCare.Model.Responses;

public class AppointmentStatusHistoryResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public AppointmentStatus OldStatus { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? ChangedByUsername { get; set; }
    public DateTime ChangedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
