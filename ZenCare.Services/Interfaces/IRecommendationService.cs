using ZenCare.Model.Responses;

namespace ZenCare.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<RecommendationItemResponse>> GetRecommendedProductsAsync(int userId, int take = 5);

        Task<List<RecommendationItemResponse>> GetRecommendedServicesAsync(int userId, int take = 5);
    }
}
