using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class PurchaseItem
{
    [Key]
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    [ForeignKey(nameof(PurchaseId))]
    public Purchase Purchase { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
}
