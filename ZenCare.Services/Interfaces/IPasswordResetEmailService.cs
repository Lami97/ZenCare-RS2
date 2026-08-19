namespace ZenCare.Services.Interfaces;

public interface IPasswordResetEmailService
{
    Task SendPasswordResetTokenAsync(string recipientEmail, string token, DateTime expiresAt);
}
