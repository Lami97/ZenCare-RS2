using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Review
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int? AppointmentId { get; set; }

    [ForeignKey(nameof(AppointmentId))]
    public Appointment? Appointment { get; set; }

    public int? ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public ReviewStatus Status { get; set; } = ReviewStatus.PendingApproval;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
