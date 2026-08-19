using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class AppointmentStatusHistory
{
    [Key]
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = null!;

    public AppointmentStatus OldStatus { get; set; }

    public AppointmentStatus NewStatus { get; set; }

    public int? ChangedByUserId { get; set; }

    [ForeignKey(nameof(ChangedByUserId))]
    public User? ChangedByUser { get; set; }

    public DateTime ChangedAt { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Reason { get; set; }
}
