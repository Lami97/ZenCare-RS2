using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IReviewService : ICRUDService<ReviewResponse, ReviewInsertRequest, ReviewUpdateRequest, ReviewSearchObject>
    {
        Task<PagedResult<ReviewResponse>> GetMyAsync(int userId, ReviewSearchObject? search);
        Task<ReviewResponse> GetMyByIdAsync(int id, int userId);
        Task<ReviewResponse> InsertMyAsync(int userId, ReviewInsertRequest request);
        Task<ReviewResponse> UpdateMyAsync(int id, int userId, ReviewUpdateRequest request);
        Task DeleteMyAsync(int id, int userId);
    }
}
