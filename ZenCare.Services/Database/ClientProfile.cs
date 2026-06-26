using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class ClientProfile
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(30)]
    public string? Gender { get; set; }

    [MaxLength(1000)]
    public string? HealthNotes { get; set; }

    [MaxLength(1000)]
    public string? Preferences { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
