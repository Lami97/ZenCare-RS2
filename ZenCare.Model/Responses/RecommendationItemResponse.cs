namespace ZenCare.Model.Responses
{
    public class RecommendationItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
