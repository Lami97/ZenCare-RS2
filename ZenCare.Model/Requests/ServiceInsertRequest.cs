using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class ServiceInsertRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int DurationMinutes { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int ServiceCategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
