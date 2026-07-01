using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ZenCareDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(ZenCareDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !VerifyPassword(user.PasswordHash, user.PasswordSalt, request.Password))
            {
                return null;
            }

            var roles = user.UserRoles
                .Select(ur => ur.Role.Name)
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .ToList();

            var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenDurationInMinutes());
            var token = GenerateToken(user, roles, expiresAt);

            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Token = token,
                ExpiresAt = expiresAt,
                Roles = roles
            };
        }

        private string GenerateToken(Database.User user, List<string> roles, DateTime expiresAt)
        {
            var issuer = _configuration["JwtToken:Issuer"];
            var audience = _configuration["JwtToken:Audience"];
            var secretKey = _configuration["JwtToken:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }

            var header = new Dictionary<string, object>
            {
                ["alg"] = "HS256",
                ["typ"] = "JWT"
            };

            var payload = new Dictionary<string, object?>
            {
                ["iss"] = issuer,
                ["aud"] = audience,
                ["exp"] = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
                ["iat"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                [ClaimTypes.NameIdentifier] = user.Id.ToString(),
                [ClaimTypes.Name] = user.Username,
                [ClaimTypes.Email] = user.Email,
                [ClaimTypes.Role] = roles
            };

            var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
            var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
            var unsignedToken = $"{encodedHeader}.{encodedPayload}";
            var signature = CreateSignature(unsignedToken, secretKey);

            return $"{unsignedToken}.{signature}";
        }

        private int GetTokenDurationInMinutes()
        {
            var configuredValue = _configuration["JwtToken:DurationInMinutes"];

            return int.TryParse(configuredValue, out var durationInMinutes) ? durationInMinutes : 60;
        }

        private static bool VerifyPassword(string passwordHash, string passwordSalt, string password)
        {
            var generatedHash = GenerateHash(password, passwordSalt);

            return passwordHash == generatedHash;
        }

        private static string GenerateHash(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(20);

            return Convert.ToBase64String(hash);
        }

        private static string CreateSignature(string unsignedToken, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));

            return Base64UrlEncode(signatureBytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
