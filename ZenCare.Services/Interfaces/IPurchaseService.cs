using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IPurchaseService : ICRUDService<PurchaseResponse, PurchaseInsertRequest, PurchaseUpdateRequest, PurchaseSearchObject>
    {
        Task<PagedResult<PurchaseResponse>> GetMyAsync(int userId, PurchaseSearchObject? search);
        Task<PurchaseResponse> GetMyByIdAsync(int id, int userId);
        Task<PurchaseResponse> InsertMyAsync(int userId, PurchaseInsertRequest request);
        Task<PurchaseResponse> UpdateMyAsync(int id, int userId, PurchaseUpdateRequest request);
        Task DeleteMyAsync(int id, int userId);
        Task<PurchaseResponse> CheckoutAsync(int userId, PurchaseCheckoutRequest request);
    }
}
