using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class PurchaseStatusHistory
{
    [Key]
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    [ForeignKey(nameof(PurchaseId))]
    public Purchase Purchase { get; set; } = null!;

    public PurchaseStatus OldStatus { get; set; }

    public PurchaseStatus NewStatus { get; set; }

    public PaymentStatus? OldPaymentStatus { get; set; }

    public PaymentStatus? NewPaymentStatus { get; set; }

    public int? ChangedByUserId { get; set; }

    [ForeignKey(nameof(ChangedByUserId))]
    public User? ChangedByUser { get; set; }

    public DateTime ChangedAt { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Reason { get; set; }
}
