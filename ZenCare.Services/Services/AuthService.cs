using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Services.Database;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Security;

namespace ZenCare.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ZenCareDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IPasswordResetEmailService _passwordResetEmailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ZenCareDbContext dbContext,
            IConfiguration configuration,
            IUserService userService,
            IPasswordResetEmailService passwordResetEmailService,
            ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
            _passwordResetEmailService = passwordResetEmailService;
            _logger = logger;
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

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            const string responseMessage = "If an account exists, password reset instructions have been sent.";
            var normalizedEmail = request.Email.Trim();
            PasswordResetToken? resetToken = null;
            string? rawToken = null;

            await using (var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable))
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.IsActive);

                if (user != null)
                {
                    var now = DateTime.UtcNow;
                    await _dbContext.PasswordResetTokens
                        .Where(token => token.UserId == user.Id && token.UsedAt == null)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now));

                    rawToken = GenerateResetToken();
                    resetToken = new PasswordResetToken
                    {
                        UserId = user.Id,
                        TokenHash = HashResetToken(rawToken),
                        CreatedAt = now,
                        ExpiresAt = now.AddMinutes(GetPasswordResetExpiryInMinutes())
                    };

                    _dbContext.PasswordResetTokens.Add(resetToken);
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }

            if (resetToken != null && rawToken != null)
            {
                try
                {
                    await _passwordResetEmailService.SendPasswordResetTokenAsync(
                        normalizedEmail,
                        rawToken,
                        resetToken.ExpiresAt);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Password reset email delivery failed ({ExceptionType}).",
                        ex.GetType().Name);

                    try
                    {
                        var invalidatedAt = DateTime.UtcNow;
                        await _dbContext.PasswordResetTokens
                            .Where(token => token.Id == resetToken.Id && token.UsedAt == null)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, invalidatedAt));
                    }
                    catch (Exception invalidationException)
                    {
                        _logger.LogError(
                            "Failed to invalidate an undelivered password reset token ({ExceptionType}).",
                            invalidationException.GetType().Name);
                    }
                }
            }

            return new ForgotPasswordResponse
            {
                Message = responseMessage
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ValidateResetPasswordRequest(request);

            var now = DateTime.UtcNow;
            var tokenHash = HashResetToken(request.Token.Trim());

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var resetToken = await _dbContext.PasswordResetTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

            if (resetToken == null || resetToken.UsedAt != null || resetToken.ExpiresAt <= now)
            {
                throw new BusinessException("The password reset token is invalid or has expired.");
            }

            var claimedRows = await _dbContext.PasswordResetTokens
                .Where(token => token.Id == resetToken.Id && token.UsedAt == null && token.ExpiresAt > now)
                .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now));

            if (claimedRows != 1)
            {
                throw new BusinessException("The password reset token is invalid or has expired.");
            }

            var user = await _dbContext.Users.FindAsync(resetToken.UserId);
            if (user == null || !user.IsActive)
            {
                throw new BusinessException("The password reset token is invalid or has expired.");
            }

            var salt = PasswordHasher.GenerateSalt();
            user.PasswordSalt = salt;
            user.PasswordHash = PasswordHasher.GenerateHash(request.NewPassword, salt);
            user.UpdatedAt = now;

            await _dbContext.PasswordResetTokens
                .Where(token => token.UserId == user.Id && token.UsedAt == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now));

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResetPasswordResponse
            {
                Message = "Password reset successfully. Please sign in."
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

        private int GetPasswordResetExpiryInMinutes()
        {
            var configuredValue = _configuration["PasswordReset:ExpiryMinutes"];

            return int.TryParse(configuredValue, out var expiryInMinutes) && expiryInMinutes > 0
                ? expiryInMinutes
                : 15;
        }

        private static string GenerateResetToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(tokenBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string HashResetToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }

        private static void ValidateResetPasswordRequest(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                throw new BusinessException("Reset token is required.");
            }

            if (string.IsNullOrEmpty(request.NewPassword))
            {
                throw new BusinessException("New password is required.");
            }

            if (request.NewPassword.Length < 6)
            {
                throw new BusinessException("New password must contain at least 6 characters.");
            }

            if (string.IsNullOrEmpty(request.ConfirmNewPassword))
            {
                throw new BusinessException("Confirm new password is required.");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new BusinessException("Passwords do not match.");
            }
        }

    }
}


