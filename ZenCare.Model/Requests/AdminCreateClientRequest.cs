using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests;

public class AdminCreateClientRequest
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string PasswordConfirm { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(30)]
    public string? Gender { get; set; }

    [MaxLength(1000)]
    public string? HealthNotes { get; set; }

    [MaxLength(1000)]
    public string? Preferences { get; set; }

    public bool IsActive { get; set; } = true;
}
