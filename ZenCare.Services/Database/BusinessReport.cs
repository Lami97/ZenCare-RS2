using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class BusinessReport
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ReportType { get; set; } = string.Empty;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int GeneratedByUserId { get; set; }

    [ForeignKey(nameof(GeneratedByUserId))]
    public User GeneratedByUser { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; }

    public int TotalAppointments { get; set; }

    public int TotalPurchases { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
