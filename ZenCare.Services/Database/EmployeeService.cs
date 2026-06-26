using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class EmployeeService
{
    [Key]
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int WellnessServiceId { get; set; }

    [ForeignKey(nameof(WellnessServiceId))]
    public WellnessService WellnessService { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
