using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class AppointmentCancelRequest
    {
        [Required]
        [MaxLength(500)]
        public string CancellationReason { get; set; } = string.Empty;
    }
}
