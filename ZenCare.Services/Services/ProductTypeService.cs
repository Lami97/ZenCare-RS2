using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ProductTypeService : BaseCRUDService<ProductTypeResponse, Database.ProductType, ProductTypeInsertRequest, ProductTypeUpdateRequest, ProductTypeSearchObject>, IProductTypeService
    {
        public ProductTypeService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.ProductType> ApplyFilters(IQueryable<Database.ProductType> query, ProductTypeSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(pt => pt.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(pt => pt.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
