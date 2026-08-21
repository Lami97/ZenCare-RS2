using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IPurchaseService : IReadService<PurchaseResponse, PurchaseSearchObject>
    {
        Task<PagedResult<PurchaseResponse>> GetMyAsync(int userId, PurchaseSearchObject? search);
        Task<PurchaseResponse> GetMyByIdAsync(int id, int userId);
        Task<PurchaseResponse> UpdateWithActorAsync(int id, int actorUserId, PurchaseUpdateRequest request);
        Task<PurchaseResponse> CheckoutAsync(int userId, PurchaseCheckoutRequest request);
        Task<PurchaseResponse> CancelMyAsync(int id, int userId);
        Task<List<PurchaseStatusHistoryResponse>> GetStatusHistoryAsync(int id);
    }
}
