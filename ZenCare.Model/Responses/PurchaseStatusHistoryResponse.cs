using ZenCare.Model.Enums;

namespace ZenCare.Model.Responses;

public class PurchaseStatusHistoryResponse
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public PurchaseStatus OldStatus { get; set; }
    public PurchaseStatus NewStatus { get; set; }
    public PaymentStatus? OldPaymentStatus { get; set; }
    public PaymentStatus? NewPaymentStatus { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? ChangedByUsername { get; set; }
    public DateTime ChangedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
