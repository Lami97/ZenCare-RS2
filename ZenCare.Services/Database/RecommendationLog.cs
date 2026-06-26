using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class RecommendationLog
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int? ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    public int? WellnessServiceId { get; set; }

    [ForeignKey(nameof(WellnessServiceId))]
    public WellnessService? WellnessService { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Score { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
