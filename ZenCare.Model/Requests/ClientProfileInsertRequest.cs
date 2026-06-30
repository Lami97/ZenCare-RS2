using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class ClientProfileInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(30)]
        public string? Gender { get; set; }

        [MaxLength(1000)]
        public string? HealthNotes { get; set; }

        [MaxLength(1000)]
        public string? Preferences { get; set; }
    }
}
