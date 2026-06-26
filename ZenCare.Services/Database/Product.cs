using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenCare.Model.Enums;

namespace ZenCare.Services.Database;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [MaxLength(50)]
    public string? SKU { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    public int ProductCategoryId { get; set; }

    [ForeignKey(nameof(ProductCategoryId))]
    public ProductCategory ProductCategory { get; set; } = null!;

    public int ProductTypeId { get; set; }

    [ForeignKey(nameof(ProductTypeId))]
    public ProductType ProductType { get; set; } = null!;

    public int UnitOfMeasureId { get; set; }

    [ForeignKey(nameof(UnitOfMeasureId))]
    public UnitOfMeasure UnitOfMeasure { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<RecommendationLog> RecommendationLogs { get; set; } = new List<RecommendationLog>();
}
