using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenCare.Services.Database;

public class FAQ
{
    [Key]
    public int Id { get; set; }

    public int FAQCategoryId { get; set; }

    [ForeignKey(nameof(FAQCategoryId))]
    public FAQCategory FAQCategory { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public string Question { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Answer { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
