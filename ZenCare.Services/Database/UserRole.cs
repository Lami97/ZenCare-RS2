using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class UserRole
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
