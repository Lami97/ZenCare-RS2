using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class AppointmentBookRequest
{
    [Range(1, int.MaxValue)]
    public int TimeSlotId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
