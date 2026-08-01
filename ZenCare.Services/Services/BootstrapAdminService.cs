using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZenCare.Model.Enums;
using ZenCare.Services.Database;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Security;

namespace ZenCare.Services.Services;

public class BootstrapAdminService : IBootstrapAdminService
{
    private readonly ZenCareDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BootstrapAdminService> _logger;

    public BootstrapAdminService(
        ZenCareDbContext dbContext,
        IConfiguration configuration,
        ILogger<BootstrapAdminService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        if (!IsBootstrapEnabled())
        {
            return;
        }

        var options = ReadOptions();
        if (options == null)
        {
            _logger.LogWarning("Bootstrap admin is enabled, but one or more required configuration values are missing.");
            return;
        }

        var adminRole = await _dbContext.Roles
            .FirstOrDefaultAsync(role => role.Name == "Admin" || role.RoleType == UserRoleType.Admin, cancellationToken);

        if (adminRole == null)
        {
            _logger.LogWarning("Bootstrap admin could not run because the Admin role was not found.");
            return;
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == options.Username || u.Email == options.Email, cancellationToken);

        if (user == null)
        {
            var salt = PasswordHasher.GenerateSalt();
            user = new User
            {
                FirstName = options.FirstName,
                LastName = options.LastName,
                Email = options.Email,
                Username = options.Username,
                PasswordSalt = salt,
                PasswordHash = PasswordHasher.GenerateHash(options.Password, salt),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Bootstrap admin user created.");
        }

        var hasAdminRole = await _dbContext.UserRoles
            .AnyAsync(userRole => userRole.UserId == user.Id && userRole.RoleId == adminRole.Id, cancellationToken);

        if (!hasAdminRole)
        {
            _dbContext.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id,
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Bootstrap admin role assignment ensured.");
        }
        else
        {
            _logger.LogInformation("Bootstrap admin already exists with Admin role.");
        }
    }

    private bool IsBootstrapEnabled()
    {
        return bool.TryParse(_configuration["BootstrapAdmin:Enabled"], out var enabled) && enabled;
    }

    private BootstrapAdminOptions? ReadOptions()
    {
        var options = new BootstrapAdminOptions(
            _configuration["BootstrapAdmin:FirstName"],
            _configuration["BootstrapAdmin:LastName"],
            _configuration["BootstrapAdmin:Email"],
            _configuration["BootstrapAdmin:Username"],
            _configuration["BootstrapAdmin:Password"]);

        return options.HasRequiredValues ? options : null;
    }

    private sealed class BootstrapAdminOptions
    {
        public BootstrapAdminOptions(string? firstName, string? lastName, string? email, string? username, string? password)
        {
            FirstName = firstName?.Trim() ?? string.Empty;
            LastName = lastName?.Trim() ?? string.Empty;
            Email = email?.Trim() ?? string.Empty;
            Username = username?.Trim() ?? string.Empty;
            Password = password ?? string.Empty;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Username { get; }
        public string Password { get; }

        public bool HasRequiredValues =>
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password);
    }
}
