using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Model.Requests
{
    public class ReviewInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        public int? AppointmentId { get; set; }

        public int? ProductId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public ReviewStatus Status { get; set; } = ReviewStatus.PendingApproval;
    }
}
