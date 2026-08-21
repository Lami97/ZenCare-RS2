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

        public override async Task<FAQResponse> InsertAsync(FAQInsertRequest request)
        {
            await EnsureFAQCategoryExistsAsync(request.FAQCategoryId);
            return await base.InsertAsync(request);
        }

        public override async Task<FAQResponse> UpdateAsync(int id, FAQUpdateRequest request)
        {
            await EnsureFAQCategoryExistsAsync(request.FAQCategoryId);
            return await base.UpdateAsync(id, request);
        }

        public override async Task<PagedResult<FAQResponse>> GetAllAsync(FAQSearchObject? search = null)
        {
            search ??= new FAQSearchObject();

            if (string.IsNullOrWhiteSpace(search.SortBy))
            {
                search.SortBy = "DisplayOrder, Question";
            }

            return await base.GetAllAsync(search);
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

        private async Task EnsureFAQCategoryExistsAsync(int faqCategoryId)
        {
            if (!await DbContext.FAQCategories.AnyAsync(category => category.Id == faqCategoryId))
            {
                throw new BusinessException("The selected FAQ category does not exist.");
            }
        }
    }
}
