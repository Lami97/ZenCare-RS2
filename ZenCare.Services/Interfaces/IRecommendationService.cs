using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<PagedResult<RecommendationItemResponse>> GetRecommendedProductsAsync(
            int userId,
            RecommendationSearchObject? search = null);

        Task<PagedResult<RecommendationItemResponse>> GetRecommendedServicesAsync(
            int userId,
            RecommendationSearchObject? search = null);
    }
}
