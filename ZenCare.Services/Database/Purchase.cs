using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Purchase
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string PurchaseNumber { get; set; } = string.Empty;

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(150)]
    public string? StripePaymentIntentId { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
