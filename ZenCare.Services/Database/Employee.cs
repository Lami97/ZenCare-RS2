using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class Employee
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [MaxLength(100)]
    public string? Specialization { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public DateTime? HireDate { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}
