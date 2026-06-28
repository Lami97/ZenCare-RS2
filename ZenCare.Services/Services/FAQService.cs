using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class FAQService : BaseCRUDService<FAQResponse, Database.FAQ, FAQInsertRequest, FAQUpdateRequest, FAQSearchObject>, IFAQService
    {
        public FAQService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<FAQResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.FAQs
                .Include(f => f.FAQCategory)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.FAQ), id);
            }

            return Mapper.Map<FAQResponse>(entity);
        }

        protected override IQueryable<Database.FAQ> ApplyFilters(IQueryable<Database.FAQ> query, FAQSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Question))
                {
                    query = query.Where(f => f.Question.Contains(search.Question));
                }

                if (search.FAQCategoryId.HasValue)
                {
                    query = query.Where(f => f.FAQCategoryId == search.FAQCategoryId.Value);
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(f => f.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.FAQ>> IncludeRelatedEntitiesAsync(IQueryable<Database.FAQ> query, FAQSearchObject? search)
        {
            query = query.Include(f => f.FAQCategory);

            return Task.FromResult(query);
        }
    }
}
