namespace ZenCare.Model.Responses
{
    public class CartItemResponse
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
