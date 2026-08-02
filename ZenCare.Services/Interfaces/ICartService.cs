using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface ICartService : ICRUDService<CartResponse, CartInsertRequest, CartUpdateRequest, CartSearchObject>
    {
        Task<CartResponse> GetMyAsync(int userId);
        Task<CartResponse> CreateMyAsync(int userId, CartInsertRequest request);
        Task<CartResponse> UpdateMyAsync(int id, int userId, CartUpdateRequest request);
        Task DeleteMyAsync(int id, int userId);
    }
}