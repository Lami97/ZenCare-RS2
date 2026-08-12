using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LogoutResponse> LogoutAsync(int userId, string jti, DateTime expiresAt);
    }
}
