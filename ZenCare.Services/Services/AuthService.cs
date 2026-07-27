using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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

    }
}
