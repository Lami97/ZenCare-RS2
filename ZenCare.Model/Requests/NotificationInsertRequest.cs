using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Model.Requests
{
    public class NotificationInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? NotificationType { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public DateTime? SentAt { get; set; }
    }
}
