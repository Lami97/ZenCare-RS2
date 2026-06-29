using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ProductService : BaseCRUDService<ProductResponse, Database.Product, ProductInsertRequest, ProductUpdateRequest, ProductSearchObject>, IProductService
    {
        public ProductService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ProductResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductType)
                .Include(p => p.UnitOfMeasure)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Product), id);
            }

            return Mapper.Map<ProductResponse>(entity);
        }

        protected override IQueryable<Database.Product> ApplyFilters(IQueryable<Database.Product> query, ProductSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(p => p.Name.Contains(search.Name));
                }

                if (search.ProductCategoryId.HasValue)
                {
                    query = query.Where(p => p.ProductCategoryId == search.ProductCategoryId.Value);
                }

                if (search.ProductTypeId.HasValue)
                {
                    query = query.Where(p => p.ProductTypeId == search.ProductTypeId.Value);
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(p => p.Status == (search.IsActive.Value ? ProductStatus.Active : ProductStatus.Inactive));
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Product>> IncludeRelatedEntitiesAsync(IQueryable<Database.Product> query, ProductSearchObject? search)
        {
            query = query
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductType)
                .Include(p => p.UnitOfMeasure);

            return Task.FromResult(query);
        }
    }
}
