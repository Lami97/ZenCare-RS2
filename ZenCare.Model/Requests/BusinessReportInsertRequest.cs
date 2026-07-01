using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class BusinessReportInsertRequest
    {
        [Required]
        [MaxLength(100)]
        public string ReportType { get; set; } = string.Empty;

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required]
        public int GeneratedByUserId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalRevenue { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalAppointments { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalPurchases { get; set; }
    }
}
