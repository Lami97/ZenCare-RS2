namespace ZenCare.Model.Messages
{
    public class PurchaseCreatedMessage
    {
        public int PurchaseId { get; set; }

        public string PurchaseNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
