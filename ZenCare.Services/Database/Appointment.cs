using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Appointment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int WellnessServiceId { get; set; }

    [ForeignKey(nameof(WellnessServiceId))]
    public WellnessService WellnessService { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
