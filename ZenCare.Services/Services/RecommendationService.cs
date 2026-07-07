using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.Services.Database;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class RecommendationService : IRecommendationService
    {
        private const int DefaultTake = 5;
        private const int MaxTake = 50;
        private const decimal CategoryPreferenceBoost = 12m;
        private const decimal TypePreferenceBoost = 8m;
        private const decimal PopularityWeight = 1m;
        private const decimal ReviewRatingWeight = 2m;
        private const decimal ReviewCountWeight = 0.5m;

        private readonly ZenCareDbContext _dbContext;

        public RecommendationService(ZenCareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<RecommendationItemResponse>> GetRecommendedProductsAsync(int userId, int take = DefaultTake)
        {
            take = NormalizeTake(take);

            var purchasedPreferences = await _dbContext.PurchaseItems
                .AsNoTracking()
                .Where(pi => pi.Purchase.UserId == userId)
                .Select(pi => new
                {
                    pi.Product.ProductCategoryId,
                    pi.Product.ProductTypeId
                })
                .Distinct()
                .ToListAsync();

            var preferredCategoryIds = purchasedPreferences.Select(x => x.ProductCategoryId).ToHashSet();
            var preferredTypeIds = purchasedPreferences.Select(x => x.ProductTypeId).ToHashSet();

            var popularityStats = await _dbContext.PurchaseItems
                .AsNoTracking()
                .GroupBy(pi => pi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(pi => pi.Quantity),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.ProductId, x => new ProductPopularityStats(x.Quantity, x.Count));

            var reviewStats = await _dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId.HasValue && r.Status == ReviewStatus.Approved)
                .GroupBy(r => r.ProductId!.Value)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.ProductId, x => new ReviewStats((decimal)x.AverageRating, x.Count));

            var products = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductType)
                .Where(p => p.Status != ProductStatus.Inactive &&
                            p.Status != ProductStatus.OutOfStock &&
                            p.Status != ProductStatus.Archived)
                .ToListAsync();

            var recommendations = products
                .Select(product => BuildProductRecommendation(product, preferredCategoryIds, preferredTypeIds, popularityStats, reviewStats))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Name)
                .Take(take)
                .ToList();

            await LogRecommendations(userId, recommendations, isProductRecommendation: true);

            return recommendations;
        }

        public async Task<List<RecommendationItemResponse>> GetRecommendedServicesAsync(int userId, int take = DefaultTake)
        {
            take = NormalizeTake(take);

            var preferredServiceCategoryIds = await _dbContext.Appointments
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Select(a => a.WellnessService.ServiceCategoryId)
                .Distinct()
                .ToListAsync();

            var preferredCategoryIds = preferredServiceCategoryIds.ToHashSet();

            var popularityStats = await _dbContext.Appointments
                .AsNoTracking()
                .GroupBy(a => a.WellnessServiceId)
                .Select(g => new
                {
                    WellnessServiceId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.WellnessServiceId, x => new PopularityStats(x.Count));

            var reviewStats = await _dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.AppointmentId.HasValue && r.Status == ReviewStatus.Approved)
                .Join(
                    _dbContext.Appointments.AsNoTracking(),
                    review => review.AppointmentId!.Value,
                    appointment => appointment.Id,
                    (review, appointment) => new
                    {
                        appointment.WellnessServiceId,
                        review.Rating
                    })
                .GroupBy(x => x.WellnessServiceId)
                .Select(g => new
                {
                    WellnessServiceId = g.Key,
                    AverageRating = g.Average(x => x.Rating),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.WellnessServiceId, x => new ReviewStats((decimal)x.AverageRating, x.Count));

            var services = await _dbContext.WellnessServices
                .AsNoTracking()
                .Include(s => s.ServiceCategory)
                .Where(s => s.Status != ServiceStatus.Inactive &&
                            s.Status != ServiceStatus.Archived)
                .ToListAsync();

            var recommendations = services
                .Select(service => BuildServiceRecommendation(service, preferredCategoryIds, popularityStats, reviewStats))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Name)
                .Take(take)
                .ToList();

            await LogRecommendations(userId, recommendations, isProductRecommendation: false);

            return recommendations;
        }

        private static RecommendationItemResponse BuildProductRecommendation(
            Product product,
            HashSet<int> preferredCategoryIds,
            HashSet<int> preferredTypeIds,
            Dictionary<int, ProductPopularityStats> popularityStats,
            Dictionary<int, ReviewStats> reviewStats)
        {
            var score = 1m;
            var reason = "Recommended as an available product.";

            if (popularityStats.TryGetValue(product.Id, out var popularity))
            {
                score += (popularity.Quantity + popularity.Count) * PopularityWeight;
                reason = "Popular product based on previous purchases.";
            }

            if (reviewStats.TryGetValue(product.Id, out var reviews))
            {
                score += (reviews.AverageRating * ReviewRatingWeight) + (reviews.Count * ReviewCountWeight);

                if (reviews.AverageRating >= 4)
                {
                    reason = "Highly rated by users.";
                }
            }

            if (preferredCategoryIds.Contains(product.ProductCategoryId))
            {
                score += CategoryPreferenceBoost;
                reason = "Recommended because you previously bought products from this category.";
            }

            if (preferredTypeIds.Contains(product.ProductTypeId))
            {
                score += TypePreferenceBoost;

                if (!preferredCategoryIds.Contains(product.ProductCategoryId))
                {
                    reason = "Recommended because it matches product types you previously purchased.";
                }
            }

            return new RecommendationItemResponse
            {
                Id = product.Id,
                Name = product.Name,
                Type = "Product",
                Score = score,
                Reason = reason
            };
        }

        private static RecommendationItemResponse BuildServiceRecommendation(
            WellnessService service,
            HashSet<int> preferredCategoryIds,
            Dictionary<int, PopularityStats> popularityStats,
            Dictionary<int, ReviewStats> reviewStats)
        {
            var score = 1m;
            var reason = "Recommended as an available service.";

            if (popularityStats.TryGetValue(service.Id, out var popularity))
            {
                score += popularity.Count * PopularityWeight;
                reason = "Popular service based on appointment history.";
            }

            if (reviewStats.TryGetValue(service.Id, out var reviews))
            {
                score += (reviews.AverageRating * ReviewRatingWeight) + (reviews.Count * ReviewCountWeight);

                if (reviews.AverageRating >= 4)
                {
                    reason = "Highly rated by users.";
                }
            }

            if (preferredCategoryIds.Contains(service.ServiceCategoryId))
            {
                score += CategoryPreferenceBoost;
                reason = "Recommended because you previously booked services from this category.";
            }

            return new RecommendationItemResponse
            {
                Id = service.Id,
                Name = service.Name,
                Type = "Service",
                Score = score,
                Reason = reason
            };
        }

        private async Task LogRecommendations(int userId, List<RecommendationItemResponse> recommendations, bool isProductRecommendation)
        {
            if (recommendations.Count == 0)
            {
                return;
            }

            var logs = recommendations.Select(recommendation => new RecommendationLog
            {
                UserId = userId,
                ProductId = isProductRecommendation ? recommendation.Id : null,
                WellnessServiceId = isProductRecommendation ? null : recommendation.Id,
                Score = recommendation.Score,
                Reason = recommendation.Reason,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            try
            {
                _dbContext.RecommendationLogs.AddRange(logs);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                foreach (var log in logs)
                {
                    _dbContext.Entry(log).State = EntityState.Detached;
                }
            }
        }

        private static int NormalizeTake(int take)
        {
            if (take <= 0)
            {
                return DefaultTake;
            }

            return Math.Min(take, MaxTake);
        }

        private sealed record ProductPopularityStats(int Quantity, int Count);

        private sealed record PopularityStats(int Count);

        private sealed record ReviewStats(decimal AverageRating, int Count);
    }
}
