using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IClientProfileService : ICRUDService<ClientProfileResponse, ClientProfileInsertRequest, ClientProfileUpdateRequest, ClientProfileSearchObject>
    {
        Task<ClientProfileResponse> GetMyAsync(int userId);
        Task<ClientProfileResponse> UpdateMyAsync(int userId, ClientProfileUpdateRequest request);
    }
}