using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class EmployeeInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public DateTime? HireDate { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
