namespace ZenCare.Model.Responses;

public class RegisterResponse
{
    public int UserId { get; set; }
    public int ClientProfileId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
