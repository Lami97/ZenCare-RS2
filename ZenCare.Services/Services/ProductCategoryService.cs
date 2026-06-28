using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ProductCategoryService : BaseCRUDService<ProductCategoryResponse, Database.ProductCategory, ProductCategoryInsertRequest, ProductCategoryUpdateRequest, ProductCategorySearchObject>, IProductCategoryService
    {
        public ProductCategoryService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.ProductCategory> ApplyFilters(IQueryable<Database.ProductCategory> query, ProductCategorySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(pc => pc.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(pc => pc.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
