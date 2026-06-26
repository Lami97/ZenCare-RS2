using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class WellnessService
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public ServiceStatus Status { get; set; } = ServiceStatus.Draft;

    public int ServiceCategoryId { get; set; }

    [ForeignKey(nameof(ServiceCategoryId))]
    public ServiceCategory ServiceCategory { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}
