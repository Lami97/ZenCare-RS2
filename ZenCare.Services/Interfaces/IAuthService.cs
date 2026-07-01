using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
