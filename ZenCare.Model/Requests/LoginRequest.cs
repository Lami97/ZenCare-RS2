using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
