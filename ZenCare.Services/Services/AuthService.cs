using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Security;

namespace ZenCare.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ZenCareDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthService(ZenCareDbContext dbContext, IConfiguration configuration, IUserService userService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !user.IsActive || !PasswordHasher.Verify(user.PasswordHash, user.PasswordSalt, request.Password))
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Token = token,
                ExpiresAt = expiresAt,
                Roles = roles
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var createdClient = await _userService.CreateClientAsync(new AdminCreateClientRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
                PasswordConfirm = request.PasswordConfirm,
                IsActive = true
            });

            return new RegisterResponse
            {
                UserId = createdClient.UserId,
                ClientProfileId = createdClient.ClientProfileId,
                Username = createdClient.Username,
                Email = createdClient.Email,
                Role = createdClient.Role,
                IsActive = createdClient.IsActive,
                CreatedAt = createdClient.CreatedAt,
                Message = "Account created successfully. Please sign in."
            };
        }

        public async Task<LogoutResponse> LogoutAsync(int userId, string jti, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                throw new BusinessException("Token cannot be invalidated.");
            }

            var alreadyRevoked = await _dbContext.RevokedTokens
                .AnyAsync(rt => rt.Jti == jti);

            if (!alreadyRevoked)
            {
                _dbContext.RevokedTokens.Add(new Database.RevokedToken
                {
                    Jti = jti,
                    UserId = userId,
                    ExpiresAt = expiresAt,
                    RevokedAt = DateTime.UtcNow
                });

                await _dbContext.SaveChangesAsync();
            }

            return new LogoutResponse
            {
                Message = "Logged out successfully."
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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

    }
}


