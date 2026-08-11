namespace ZenCare.Model.SearchObjects
{
    public class RecommendationLogSearchObject : PagedSearchObject
    {
        public int? UserId { get; set; }

        public int? ProductId { get; set; }

        public int? WellnessServiceId { get; set; }
    }
}
