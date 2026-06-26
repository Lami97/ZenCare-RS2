using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    public int CartId { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart Cart { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
