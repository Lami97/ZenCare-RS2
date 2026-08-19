using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services;

public class SmtpPasswordResetEmailService : IPasswordResetEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpPasswordResetEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetTokenAsync(string recipientEmail, string token, DateTime expiresAt)
    {
        var host = _configuration["Smtp:Host"];
        var fromAddress = _configuration["Smtp:FromAddress"];
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromAddress))
        {
            throw new InvalidOperationException("SMTP configuration is incomplete.");
        }

        if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("SMTP credentials are incomplete.");
        }

        var port = int.TryParse(_configuration["Smtp:Port"], out var configuredPort)
            ? configuredPort
            : 587;
        var useSsl = !bool.TryParse(_configuration["Smtp:UseSsl"], out var configuredUseSsl)
            || configuredUseSsl;
        var fromName = _configuration["Smtp:FromName"] ?? "ZenCare";

        using var message = new MailMessage
        {
            From = new MailAddress(fromAddress, fromName),
            Subject = "ZenCare password reset",
            Body = $"Use this password reset token in the ZenCare application:\n\n{token}\n\n"
                + $"The token expires at {expiresAt:O}.",
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(recipientEmail));

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = useSsl
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            client.Credentials = new NetworkCredential(username, password);
        }

        await client.SendMailAsync(message);
    }
}
