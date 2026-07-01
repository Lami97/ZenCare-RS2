namespace ZenCare.Model.Responses
{
    public class RecommendationLogResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? WellnessServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal Score { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
