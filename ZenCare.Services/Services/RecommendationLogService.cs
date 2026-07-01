using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class RecommendationLogService : BaseCRUDService<RecommendationLogResponse, Database.RecommendationLog, RecommendationLogInsertRequest, RecommendationLogUpdateRequest, RecommendationLogSearchObject>, IRecommendationLogService
    {
        public RecommendationLogService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<RecommendationLogResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.RecommendationLogs
                .Include(rl => rl.User)
                .Include(rl => rl.Product)
                .Include(rl => rl.WellnessService)
                .FirstOrDefaultAsync(rl => rl.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.RecommendationLog), id);
            }

            return Mapper.Map<RecommendationLogResponse>(entity);
        }

        protected override IQueryable<Database.RecommendationLog> ApplyFilters(IQueryable<Database.RecommendationLog> query, RecommendationLogSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(rl => rl.UserId == search.UserId.Value);
                }

                if (search.ProductId.HasValue)
                {
                    query = query.Where(rl => rl.ProductId == search.ProductId.Value);
                }

                if (search.WellnessServiceId.HasValue)
                {
                    query = query.Where(rl => rl.WellnessServiceId == search.WellnessServiceId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.RecommendationLog>> IncludeRelatedEntitiesAsync(IQueryable<Database.RecommendationLog> query, RecommendationLogSearchObject? search)
        {
            query = query
                .Include(rl => rl.User)
                .Include(rl => rl.Product)
                .Include(rl => rl.WellnessService);

            return Task.FromResult(query);
        }
    }
}
