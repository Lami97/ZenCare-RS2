using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class UpdateMyProfileRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name must not exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name must not exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    [EmailAddress(ErrorMessage = "Email must be in the format: user@example.com.")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(10, ErrorMessage = "Phone number must contain 9 or 10 digits (numbers only).")]
    [RegularExpression(@"^\d{9,10}$", ErrorMessage = "Phone number must contain 9 or 10 digits (numbers only).")]
    public string? PhoneNumber { get; set; }
}
