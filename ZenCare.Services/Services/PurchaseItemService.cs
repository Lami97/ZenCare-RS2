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
    public class PurchaseItemService : BaseCRUDService<PurchaseItemResponse, Database.PurchaseItem, PurchaseItemInsertRequest, PurchaseItemUpdateRequest, PurchaseItemSearchObject>, IPurchaseItemService
    {
        public PurchaseItemService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<PurchaseItemResponse> InsertAsync(PurchaseItemInsertRequest request)
        {
            await PreparePurchaseItemRequestAsync(request);

            var response = await base.InsertAsync(request);
            await UpdatePurchaseTotalAsync(request.PurchaseId);

            return await GetByIdAsync(response.Id);
        }

        public override async Task<PurchaseItemResponse> UpdateAsync(int id, PurchaseItemUpdateRequest request)
        {
            var existingEntity = await DbContext.PurchaseItems
                .AsNoTracking()
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException(nameof(Database.PurchaseItem), id);
            }

            await PreparePurchaseItemRequestAsync(request);

            var response = await base.UpdateAsync(id, request);

            if (existingEntity.PurchaseId != request.PurchaseId)
            {
                await UpdatePurchaseTotalAsync(existingEntity.PurchaseId);
            }

            await UpdatePurchaseTotalAsync(request.PurchaseId);

            return await GetByIdAsync(response.Id);
        }

        public override async Task DeleteAsync(int id)
        {
            var existingEntity = await DbContext.PurchaseItems
                .AsNoTracking()
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException(nameof(Database.PurchaseItem), id);
            }

            await base.DeleteAsync(id);
            await UpdatePurchaseTotalAsync(existingEntity.PurchaseId);
        }

        public async Task<PagedResult<PurchaseItemResponse>> GetMyAsync(int userId, PurchaseItemSearchObject? search)
        {
            var query = GetOwnedPurchaseItemQuery(userId);
            query = ApplyFilters(query, search);

            return await CreatePagedResultAsync(query, search);
        }

        public async Task<PurchaseItemResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetOwnedPurchaseItemQuery(userId)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (entity == null)
            {
                throw new BusinessException("Purchase item was not found.");
            }

            return Mapper.Map<PurchaseItemResponse>(entity);
        }

        public override async Task<PurchaseItemResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.PurchaseItems
                .Include(pi => pi.Purchase)
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.PurchaseItem), id);
            }

            return Mapper.Map<PurchaseItemResponse>(entity);
        }

        protected override IQueryable<Database.PurchaseItem> ApplyFilters(IQueryable<Database.PurchaseItem> query, PurchaseItemSearchObject? search)
        {
            if (search != null)
            {
                if (search.PurchaseId.HasValue)
                {
                    query = query.Where(pi => pi.PurchaseId == search.PurchaseId.Value);
                }

                if (search.ProductId.HasValue)
                {
                    query = query.Where(pi => pi.ProductId == search.ProductId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.PurchaseItem>> IncludeRelatedEntitiesAsync(IQueryable<Database.PurchaseItem> query, PurchaseItemSearchObject? search)
        {
            query = query
                .Include(pi => pi.Purchase)
                .Include(pi => pi.Product);

            return Task.FromResult(query);
        }

        private IQueryable<Database.PurchaseItem> GetOwnedPurchaseItemQuery(int userId)
        {
            return DbContext.PurchaseItems
                .Include(pi => pi.Purchase)
                .Include(pi => pi.Product)
                .Where(pi => pi.Purchase.UserId == userId);
        }

        private async Task PreparePurchaseItemRequestAsync(PurchaseItemInsertRequest request)
        {
            await PreparePurchaseItemRequestAsync(request.PurchaseId, request.ProductId, request.Quantity, (unitPrice, totalPrice) =>
            {
                request.UnitPrice = unitPrice;
                request.TotalPrice = totalPrice;
            });
        }

        private async Task PreparePurchaseItemRequestAsync(PurchaseItemUpdateRequest request)
        {
            await PreparePurchaseItemRequestAsync(request.PurchaseId, request.ProductId, request.Quantity, (unitPrice, totalPrice) =>
            {
                request.UnitPrice = unitPrice;
                request.TotalPrice = totalPrice;
            });
        }

        private async Task PreparePurchaseItemRequestAsync(int purchaseId, int productId, int quantity, Action<decimal, decimal> setCalculatedPrices)
        {
            if (quantity <= 0)
            {
                throw new BusinessException("Quantity must be greater than zero.");
            }

            var purchaseExists = await DbContext.Purchases
                .AnyAsync(p => p.Id == purchaseId);

            if (!purchaseExists)
            {
                throw new BusinessException("Purchase was not found.");
            }

            var product = await DbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new BusinessException("Product was not found.");
            }

            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessException("Product must be active.");
            }

            setCalculatedPrices(product.Price, product.Price * quantity);
        }

        private async Task UpdatePurchaseTotalAsync(int purchaseId)
        {
            var purchase = await DbContext.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
            {
                return;
            }

            purchase.TotalAmount = await DbContext.PurchaseItems
                .Where(pi => pi.PurchaseId == purchaseId)
                .SumAsync(pi => pi.TotalPrice);
            purchase.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();
        }

        private async Task<PagedResult<PurchaseItemResponse>> CreatePagedResultAsync(IQueryable<Database.PurchaseItem> query, PurchaseItemSearchObject? search)
        {
            int? totalCount = null;

            if (search?.IncludeTotalCount == true)
            {
                totalCount = await query.CountAsync();
            }

            query = query.OrderBy(pi => pi.Id);
            query = ApplyPagination(query, search);

            var entities = await query.ToListAsync();

            return new PagedResult<PurchaseItemResponse>
            {
                Items = Mapper.Map<List<PurchaseItemResponse>>(entities),
                TotalCount = totalCount
            };
        }
    }
}
