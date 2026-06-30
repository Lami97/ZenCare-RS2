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
    }
}
