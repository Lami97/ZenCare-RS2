using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ReviewService : BaseCRUDService<ReviewResponse, Database.Review, ReviewInsertRequest, ReviewUpdateRequest, ReviewSearchObject>, IReviewService
    {
        public ReviewService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<PagedResult<ReviewResponse>> GetMyAsync(int userId, ReviewSearchObject? search)
        {
            search ??= new ReviewSearchObject();
            search.UserId = userId;

            return await GetAllAsync(search);
        }

        public async Task<ReviewResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetClientReviewEntityAsync(id, userId);

            return Mapper.Map<ReviewResponse>(entity);
        }

        public async Task<ReviewResponse> InsertMyAsync(int userId, ReviewInsertRequest request)
        {
            request.UserId = userId;

            return await InsertAsync(request);
        }

        public async Task<ReviewResponse> UpdateMyAsync(int id, int userId, ReviewUpdateRequest request)
        {
            await EnsureClientReviewExistsAsync(id, userId);

            request.Id = id;
            request.UserId = userId;

            return await UpdateAsync(id, request);
        }

        public async Task DeleteMyAsync(int id, int userId)
        {
            var entity = await DbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            DbContext.Reviews.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task<ReviewResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            return Mapper.Map<ReviewResponse>(entity);
        }

        protected override IQueryable<Database.Review> ApplyFilters(IQueryable<Database.Review> query, ReviewSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(r => r.UserId == search.UserId.Value);
                }

                if (search.AppointmentId.HasValue)
                {
                    query = query.Where(r => r.AppointmentId == search.AppointmentId.Value);
                }

                if (search.ProductId.HasValue)
                {
                    query = query.Where(r => r.ProductId == search.ProductId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(r => r.Status == search.Status.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Review>> IncludeRelatedEntitiesAsync(IQueryable<Database.Review> query, ReviewSearchObject? search)
        {
            query = query
                .Include(r => r.User)
                .Include(r => r.Product);

            return Task.FromResult(query);
        }

        private async Task<Database.Review> GetClientReviewEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            return entity;
        }

        private async Task EnsureClientReviewExistsAsync(int id, int userId)
        {
            var exists = await DbContext.Reviews
                .AnyAsync(r => r.Id == id && r.UserId == userId);

            if (!exists)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }
        }
    }
}
