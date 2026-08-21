using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductService> _logger;

        public ProductService(ZenCareDbContext dbContext, IMapper mapper, ILogger<ProductService> logger) : base(dbContext, mapper)
        {
            _logger = logger;
        }

        public override async Task<ProductResponse> InsertAsync(ProductInsertRequest request)
        {
            await ValidateReferencesAsync(
                request.ProductCategoryId,
                request.ProductTypeId,
                request.UnitOfMeasureId,
                request.SupplierId);

            var entity = Mapper.Map<Database.Product>(request);
            entity.CreatedAt = DateTime.UtcNow;

            DbContext.Products.Add(entity);
            await DbContext.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public override async Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request)
        {
            var entity = await DbContext.Products.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Product), id);
            }

            await ValidateReferencesAsync(
                request.ProductCategoryId,
                request.ProductTypeId,
                request.UnitOfMeasureId,
                request.SupplierId);

            Mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await DbContext.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public override async Task<ProductResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductType)
                .Include(p => p.UnitOfMeasure)
                .Include(p => p.Supplier)
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

                if (search.SupplierId.HasValue)
                {
                    query = query.Where(p => p.SupplierId == search.SupplierId.Value);
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
                .Include(p => p.UnitOfMeasure)
                .Include(p => p.Supplier);

            return Task.FromResult(query);
        }

        public async Task RecordMyViewAsync(int productId, int userId)
        {
            var productExists = await DbContext.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == productId && p.Status == ProductStatus.Active);

            if (!productExists)
            {
                throw new NotFoundException(nameof(Database.Product), productId);
            }

            try
            {
                var viewedAt = DateTime.UtcNow;
                var productView = await DbContext.ProductViews
                    .FirstOrDefaultAsync(pv => pv.UserId == userId && pv.ProductId == productId);

                if (productView == null)
                {
                    DbContext.ProductViews.Add(new Database.ProductView
                    {
                        UserId = userId,
                        ProductId = productId,
                        ViewCount = 1,
                        LastViewedAt = viewedAt
                    });
                }
                else
                {
                    if (productView.ViewCount < int.MaxValue)
                    {
                        productView.ViewCount++;
                    }

                    productView.LastViewedAt = viewedAt;
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Could not record product view for user {UserId} and product {ProductId}.",
                    userId,
                    productId);
            }
        }

        private async Task ValidateReferencesAsync(
            int productCategoryId,
            int productTypeId,
            int unitOfMeasureId,
            int supplierId)
        {
            if (!await DbContext.ProductCategories.AnyAsync(category => category.Id == productCategoryId))
            {
                throw new BusinessException("The selected product category does not exist.");
            }

            if (!await DbContext.ProductTypes.AnyAsync(type => type.Id == productTypeId))
            {
                throw new BusinessException("The selected product type does not exist.");
            }

            if (!await DbContext.UnitOfMeasures.AnyAsync(unit => unit.Id == unitOfMeasureId))
            {
                throw new BusinessException("The selected unit of measure does not exist.");
            }

            if (!await DbContext.Suppliers.AnyAsync(s => s.Id == supplierId))
            {
                throw new BusinessException("The selected supplier does not exist.");
            }
        }
    }
}
