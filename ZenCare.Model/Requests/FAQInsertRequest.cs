using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class FAQInsertRequest
    {
        [Required]
        [MaxLength(250)]
        public string Question { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Answer { get; set; } = string.Empty;

        [Required]
        public int FAQCategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
