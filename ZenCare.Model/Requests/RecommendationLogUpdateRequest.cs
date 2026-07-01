using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class RecommendationLogUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? ProductId { get; set; }

        public int? WellnessServiceId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Score { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
