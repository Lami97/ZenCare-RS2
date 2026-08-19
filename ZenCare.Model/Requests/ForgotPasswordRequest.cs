using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    [EmailAddress(ErrorMessage = "Email must be in the format: user@example.com.")]
    public string Email { get; set; } = string.Empty;
}
