using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class ProductInsertRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        public int UnitOfMeasureId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
