using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class RevokedToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Jti { get; set; } = string.Empty;

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
}
