using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IPurchaseItemService : ICRUDService<PurchaseItemResponse, PurchaseItemInsertRequest, PurchaseItemUpdateRequest, PurchaseItemSearchObject>
    {
        Task<PagedResult<PurchaseItemResponse>> GetMyAsync(int userId, PurchaseItemSearchObject? search);
        Task<PurchaseItemResponse> GetMyByIdAsync(int id, int userId);
    }
}