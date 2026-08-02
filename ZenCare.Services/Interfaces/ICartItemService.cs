using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface ICartItemService : ICRUDService<CartItemResponse, CartItemInsertRequest, CartItemUpdateRequest, CartItemSearchObject>
    {
        Task<PagedResult<CartItemResponse>> GetMyAsync(int userId, CartItemSearchObject? search);
        Task<CartItemResponse> GetMyByIdAsync(int id, int userId);
        Task<CartItemResponse> CreateMyAsync(int userId, CartItemInsertRequest request);
        Task<CartItemResponse> UpdateMyAsync(int id, int userId, CartItemUpdateRequest request);
        Task DeleteMyAsync(int id, int userId);
    }
}