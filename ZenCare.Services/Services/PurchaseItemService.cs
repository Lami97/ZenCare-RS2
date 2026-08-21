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
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            ValidateQuantity(request.Quantity);

            var purchase = await GetMutablePurchaseAsync(request.PurchaseId);
            var product = await GetActiveProductAsync(request.ProductId);

            await EnsureProductIsNotAlreadyIncludedAsync(request.PurchaseId, request.ProductId);
            ValidateAvailableStock(product, request.Quantity);

            var entity = new Database.PurchaseItem
            {
                PurchaseId = purchase.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * request.Quantity
            };

            product.StockQuantity -= request.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            DbContext.PurchaseItems.Add(entity);
            await DbContext.SaveChangesAsync();

            await RecalculatePurchaseTotalAsync(purchase.Id);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(entity.Id);
        }

        public override async Task<PurchaseItemResponse> UpdateAsync(int id, PurchaseItemUpdateRequest request)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var existingEntity = await DbContext.PurchaseItems
                .Include(pi => pi.Purchase)
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException(nameof(Database.PurchaseItem), id);
            }

            ValidateQuantity(request.Quantity);
            await ValidatePurchaseCanBeModifiedAsync(existingEntity.Purchase);

            var targetPurchase = existingEntity.PurchaseId == request.PurchaseId
                ? existingEntity.Purchase
                : await GetMutablePurchaseAsync(request.PurchaseId);

            await EnsureProductIsNotAlreadyIncludedAsync(request.PurchaseId, request.ProductId, id);

            var targetProduct = existingEntity.ProductId == request.ProductId
                ? existingEntity.Product
                : await GetActiveProductAsync(request.ProductId);

            ValidateProductIsActive(targetProduct);

            var productChanged = existingEntity.ProductId != request.ProductId;
            var unitPrice = productChanged ? targetProduct.Price : existingEntity.UnitPrice;

            if (productChanged)
            {
                ValidateAvailableStock(targetProduct, request.Quantity);

                existingEntity.Product.StockQuantity += existingEntity.Quantity;
                existingEntity.Product.UpdatedAt = DateTime.UtcNow;

                targetProduct.StockQuantity -= request.Quantity;
                targetProduct.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var quantityDelta = request.Quantity - existingEntity.Quantity;

                if (quantityDelta > 0)
                {
                    ValidateAvailableStock(targetProduct, quantityDelta);
                }

                targetProduct.StockQuantity -= quantityDelta;
                if (quantityDelta != 0)
                {
                    targetProduct.UpdatedAt = DateTime.UtcNow;
                }
            }

            var originalPurchaseId = existingEntity.PurchaseId;

            existingEntity.PurchaseId = targetPurchase.Id;
            existingEntity.Purchase = targetPurchase;
            existingEntity.ProductId = targetProduct.Id;
            existingEntity.Product = targetProduct;
            existingEntity.Quantity = request.Quantity;
            existingEntity.UnitPrice = unitPrice;
            existingEntity.TotalPrice = unitPrice * request.Quantity;

            await DbContext.SaveChangesAsync();

            if (originalPurchaseId != targetPurchase.Id)
            {
                await RecalculatePurchaseTotalAsync(originalPurchaseId);
            }

            await RecalculatePurchaseTotalAsync(targetPurchase.Id);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(existingEntity.Id);
        }

        public override async Task DeleteAsync(int id)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var existingEntity = await DbContext.PurchaseItems
                .Include(pi => pi.Purchase)
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException(nameof(Database.PurchaseItem), id);
            }

            await ValidatePurchaseCanBeModifiedAsync(existingEntity.Purchase);

            var purchaseId = existingEntity.PurchaseId;
            existingEntity.Product.StockQuantity += existingEntity.Quantity;
            existingEntity.Product.UpdatedAt = DateTime.UtcNow;

            DbContext.PurchaseItems.Remove(existingEntity);
            await DbContext.SaveChangesAsync();

            await RecalculatePurchaseTotalAsync(purchaseId);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();
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

        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessException("Quantity must be greater than zero.");
            }
        }

        private async Task<Database.Purchase> GetMutablePurchaseAsync(int purchaseId)
        {
            var purchase = await DbContext.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
            {
                throw new BusinessException("Purchase was not found.");
            }

            await ValidatePurchaseCanBeModifiedAsync(purchase);
            return purchase;
        }

        private async Task ValidatePurchaseCanBeModifiedAsync(Database.Purchase purchase)
        {
            var paymentProcessingStarted = !string.IsNullOrWhiteSpace(purchase.StripePaymentIntentId) ||
                await DbContext.Payments.AnyAsync(payment =>
                    payment.PurchaseId == purchase.Id &&
                    payment.StripePaymentIntentId != null &&
                    payment.StripePaymentIntentId != string.Empty);

            if ((purchase.Status is not PurchaseStatus.Draft and not PurchaseStatus.PendingPayment) ||
                paymentProcessingStarted)
            {
                throw new BusinessException("Purchase items cannot be modified after payment processing has started.");
            }
        }

        private async Task<Database.Product> GetActiveProductAsync(int productId)
        {
            var product = await DbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new BusinessException("Product was not found.");
            }

            ValidateProductIsActive(product);
            return product;
        }

        private static void ValidateProductIsActive(Database.Product product)
        {
            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessException("Product must be active.");
            }
        }

        private static void ValidateAvailableStock(Database.Product product, int requiredQuantity)
        {
            if (product.StockQuantity < requiredQuantity)
            {
                throw new BusinessException("Not enough stock is available for the selected product.");
            }
        }

        private async Task EnsureProductIsNotAlreadyIncludedAsync(int purchaseId, int productId, int? excludedItemId = null)
        {
            var duplicateExists = await DbContext.PurchaseItems.AnyAsync(pi =>
                pi.PurchaseId == purchaseId &&
                pi.ProductId == productId &&
                (!excludedItemId.HasValue || pi.Id != excludedItemId.Value));

            if (duplicateExists)
            {
                throw new BusinessException("This product is already included in the purchase.");
            }
        }

        private async Task RecalculatePurchaseTotalAsync(int purchaseId)
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
