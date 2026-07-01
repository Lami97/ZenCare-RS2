namespace ZenCare.Model.Responses
{
    public class PurchaseItemResponse
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
