using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class TimeSlot
{
    [Key]
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int WellnessServiceId { get; set; }

    [ForeignKey(nameof(WellnessServiceId))]
    public WellnessService WellnessService { get; set; } = null!;

    public DateTime SlotDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
