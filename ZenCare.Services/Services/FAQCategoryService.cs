using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class FAQCategoryService : BaseCRUDService<FAQCategoryResponse, Database.FAQCategory, FAQCategoryInsertRequest, FAQCategoryUpdateRequest, FAQCategorySearchObject>, IFAQCategoryService
    {
        public FAQCategoryService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.FAQCategory> ApplyFilters(IQueryable<Database.FAQCategory> query, FAQCategorySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(fc => fc.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(fc => fc.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
