using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public UserRoleType RoleType { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
