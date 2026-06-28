using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ServiceCategoryService : BaseCRUDService<ServiceCategoryResponse, Database.ServiceCategory, ServiceCategoryInsertRequest, ServiceCategoryUpdateRequest, ServiceCategorySearchObject>, IServiceCategoryService
    {
        public ServiceCategoryService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.ServiceCategory> ApplyFilters(IQueryable<Database.ServiceCategory> query, ServiceCategorySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(sc => sc.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(sc => sc.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
