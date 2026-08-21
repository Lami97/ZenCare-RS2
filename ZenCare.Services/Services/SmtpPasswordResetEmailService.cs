using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ZenCare.Services.Configuration;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services;

public class SmtpPasswordResetEmailService : IPasswordResetEmailService
{
    private readonly string? _host;
    private readonly int _port;
    private readonly string? _username;
    private readonly string? _password;
    private readonly bool _useSsl;
    private readonly string? _fromAddress;
    private readonly string _fromName;

    public SmtpPasswordResetEmailService(IOptions<SmtpOptions> options)
    {
        var settings = options.Value;
        _host = settings.Host;
        _port = int.TryParse(settings.Port, out var configuredPort) ? configuredPort : 587;
        _username = settings.Username;
        _password = settings.Password;
        _useSsl = !bool.TryParse(settings.UseSsl, out var configuredUseSsl) || configuredUseSsl;
        _fromAddress = settings.FromAddress;
        _fromName = settings.FromName ?? "ZenCare";
    }

    public async Task SendPasswordResetTokenAsync(string recipientEmail, string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(_host) || string.IsNullOrWhiteSpace(_fromAddress))
        {
            throw new InvalidOperationException("SMTP configuration is incomplete.");
        }

        if (!string.IsNullOrWhiteSpace(_username) && string.IsNullOrWhiteSpace(_password))
        {
            throw new InvalidOperationException("SMTP credentials are incomplete.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_fromAddress, _fromName),
            Subject = "ZenCare password reset",
            Body = $"Use this password reset token in the ZenCare application:\n\n{token}\n\n"
                + $"The token expires at {expiresAt:O}.",
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(recipientEmail));

        using var client = new SmtpClient(_host, _port)
        {
            EnableSsl = _useSsl
        };

        if (!string.IsNullOrWhiteSpace(_username))
        {
            client.Credentials = new NetworkCredential(_username, _password);
        }

        await client.SendMailAsync(message);
    }
}
