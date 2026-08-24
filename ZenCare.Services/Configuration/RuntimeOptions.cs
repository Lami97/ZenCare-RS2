namespace ZenCare.Services.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "JwtToken";

    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string? SecretKey { get; init; }
    public string? DurationInMinutes { get; init; }
}

public sealed class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    public string? ExpiryMinutes { get; init; }
}

public sealed class StripeOptions
{
    public const string SectionName = "Stripe";

    public string? SecretKey { get; init; }
    public string? PublishableKey { get; init; }
    public string? Currency { get; init; }
}

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string? Host { get; init; }
    public string? Port { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string? Host { get; init; }
    public string? Port { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? UseSsl { get; init; }
    public string? FromAddress { get; init; }
    public string? FromName { get; init; }
}
