using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Notification
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NotificationType { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public DateTime? SentAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
