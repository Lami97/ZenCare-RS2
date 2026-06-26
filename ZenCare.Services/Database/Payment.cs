using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Payment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int? AppointmentId { get; set; }

    [ForeignKey(nameof(AppointmentId))]
    public Appointment? Appointment { get; set; }

    public int? PurchaseId { get; set; }

    [ForeignKey(nameof(PurchaseId))]
    public Purchase? Purchase { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [MaxLength(150)]
    public string? StripePaymentIntentId { get; set; }

    [MaxLength(150)]
    public string? StripeChargeId { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
