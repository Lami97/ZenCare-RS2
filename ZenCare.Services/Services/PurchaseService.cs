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
    public class PurchaseService : BaseCRUDService<PurchaseResponse, Database.Purchase, PurchaseInsertRequest, PurchaseUpdateRequest, PurchaseSearchObject>, IPurchaseService
    {
        public PurchaseService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<PurchaseResponse> InsertAsync(PurchaseInsertRequest request)
        {
            ValidatePurchaseRequest(request.PurchaseNumber, request.TotalAmount, request.Status, request.PaymentStatus);

            return await base.InsertAsync(request);
        }

        public override async Task<PurchaseResponse> UpdateAsync(int id, PurchaseUpdateRequest request)
        {
            var entity = await DbContext.Purchases.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            ValidatePurchaseRequest(request.PurchaseNumber, request.TotalAmount, request.Status, request.PaymentStatus);
            ValidatePurchaseStatusTransition(entity.Status, request.Status);

            return await base.UpdateAsync(id, request);
        }

        public async Task<PagedResult<PurchaseResponse>> GetMyAsync(int userId, PurchaseSearchObject? search)
        {
            search ??= new PurchaseSearchObject();
            search.UserId = userId;

            return await GetAllAsync(search);
        }

        public async Task<PurchaseResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetClientPurchaseEntityAsync(id, userId);

            return Mapper.Map<PurchaseResponse>(entity);
        }

        public async Task<PurchaseResponse> InsertMyAsync(int userId, PurchaseInsertRequest request)
        {
            request.UserId = userId;

            return await InsertAsync(request);
        }

        public async Task<PurchaseResponse> UpdateMyAsync(int id, int userId, PurchaseUpdateRequest request)
        {
            await EnsureClientPurchaseExistsAsync(id, userId);

            request.Id = id;
            request.UserId = userId;

            return await UpdateAsync(id, request);
        }

        public async Task DeleteMyAsync(int id, int userId)
        {
            var entity = await DbContext.Purchases
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            DbContext.Purchases.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public async Task<PurchaseResponse> CheckoutAsync(int userId, PurchaseCheckoutRequest request)
        {
            ValidateCheckoutRequest(request);

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var requestedProductIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await DbContext.Products
                .Where(p => requestedProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var purchaseItems = new List<Database.PurchaseItem>();
            var totalAmount = 0m;

            foreach (var item in request.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new BusinessException("Product was not found.");
                }

                ValidateProductForCheckout(product, item.Quantity);

                var unitPrice = product.Price;
                var totalPrice = unitPrice * item.Quantity;

                totalAmount += totalPrice;
                product.StockQuantity -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                purchaseItems.Add(new Database.PurchaseItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            var purchase = new Database.Purchase
            {
                UserId = userId,
                PurchaseNumber = await GeneratePurchaseNumberAsync(),
                Status = PurchaseStatus.PendingPayment,
                PaymentStatus = PaymentStatus.Pending,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                PurchaseItems = purchaseItems
            };

            DbContext.Purchases.Add(purchase);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(purchase.Id);
        }

        public override async Task<PurchaseResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Purchases
                .Include(p => p.User)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            return Mapper.Map<PurchaseResponse>(entity);
        }

        protected override IQueryable<Database.Purchase> ApplyFilters(IQueryable<Database.Purchase> query, PurchaseSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(p => p.UserId == search.UserId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(p => p.Status == search.Status.Value);
                }

                if (search.PaymentStatus.HasValue)
                {
                    query = query.Where(p => p.PaymentStatus == search.PaymentStatus.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.PurchaseNumber))
                {
                    query = query.Where(p => p.PurchaseNumber.Contains(search.PurchaseNumber));
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Purchase>> IncludeRelatedEntitiesAsync(IQueryable<Database.Purchase> query, PurchaseSearchObject? search)
        {
            query = query
                .Include(p => p.User)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product);

            return Task.FromResult(query);
        }

        private async Task<Database.Purchase> GetClientPurchaseEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Purchases
                .Include(p => p.User)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            return entity;
        }

        private async Task EnsureClientPurchaseExistsAsync(int id, int userId)
        {
            var exists = await DbContext.Purchases
                .AnyAsync(p => p.Id == id && p.UserId == userId);

            if (!exists)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }
        }

        private static void ValidateCheckoutRequest(PurchaseCheckoutRequest request)
        {
            if (request.Items.Count == 0)
            {
                throw new BusinessException("Checkout items are required.");
            }

            if (request.Items.Any(i => i.Quantity <= 0))
            {
                throw new BusinessException("Quantity must be greater than zero.");
            }

            var hasDuplicateProducts = request.Items
                .GroupBy(i => i.ProductId)
                .Any(g => g.Count() > 1);

            if (hasDuplicateProducts)
            {
                throw new BusinessException("Duplicate products are not allowed in one checkout request.");
            }
        }

        private static void ValidateProductForCheckout(Database.Product product, int quantity)
        {
            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessException("Product must be active.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new BusinessException("Not enough stock is available for the selected product.");
            }
        }

        private async Task<string> GeneratePurchaseNumberAsync()
        {
            string purchaseNumber;

            do
            {
                purchaseNumber = $"PC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }
            while (await DbContext.Purchases.AnyAsync(p => p.PurchaseNumber == purchaseNumber));

            return purchaseNumber;
        }

        private static void ValidatePurchaseRequest(string? purchaseNumber, decimal totalAmount, PurchaseStatus status, PaymentStatus paymentStatus)
        {
            if (string.IsNullOrWhiteSpace(purchaseNumber))
            {
                throw new BusinessException("Purchase number is required.");
            }

            if (totalAmount < 0)
            {
                throw new BusinessException("Total amount must be greater than or equal to zero.");
            }

            if (!Enum.IsDefined(typeof(PurchaseStatus), status))
            {
                throw new BusinessException("Purchase status is not valid.");
            }

            if (!Enum.IsDefined(typeof(PaymentStatus), paymentStatus))
            {
                throw new BusinessException("Payment status is not valid.");
            }

            ValidatePaymentStatus(status, paymentStatus);
        }

        private static void ValidatePurchaseStatusTransition(PurchaseStatus currentStatus, PurchaseStatus newStatus)
        {
            if (currentStatus == newStatus)
            {
                if (IsTerminalStatus(currentStatus))
                {
                    throw new BusinessException(GetTerminalStatusMessage(currentStatus));
                }

                return;
            }

            if (IsTerminalStatus(currentStatus))
            {
                throw new BusinessException(GetTerminalStatusMessage(currentStatus));
            }

            if (!IsValidTransition(currentStatus, newStatus))
            {
                throw new BusinessException("Invalid purchase status transition.");
            }
        }

        private static bool IsValidTransition(PurchaseStatus currentStatus, PurchaseStatus newStatus)
        {
            return currentStatus switch
            {
                PurchaseStatus.Draft => newStatus == PurchaseStatus.PendingPayment,
                PurchaseStatus.PendingPayment => newStatus is PurchaseStatus.Paid or PurchaseStatus.Cancelled or PurchaseStatus.Failed,
                PurchaseStatus.Paid => newStatus is PurchaseStatus.Processing or PurchaseStatus.Refunded,
                PurchaseStatus.Processing => newStatus is PurchaseStatus.ReadyForPickup or PurchaseStatus.Shipped,
                PurchaseStatus.ReadyForPickup => newStatus == PurchaseStatus.Completed,
                PurchaseStatus.Shipped => newStatus == PurchaseStatus.Completed,
                _ => false
            };
        }

        private static bool IsTerminalStatus(PurchaseStatus status)
        {
            return status is PurchaseStatus.Completed or PurchaseStatus.Cancelled or PurchaseStatus.Refunded or PurchaseStatus.Failed;
        }

        private static string GetTerminalStatusMessage(PurchaseStatus status)
        {
            return status switch
            {
                PurchaseStatus.Completed => "Completed purchases cannot be modified.",
                PurchaseStatus.Refunded => "Refunded purchases cannot be reactivated.",
                PurchaseStatus.Cancelled => "Cancelled purchases cannot be modified.",
                PurchaseStatus.Failed => "Failed purchases cannot be modified.",
                _ => "Terminal purchases cannot be modified."
            };
        }

        private static void ValidatePaymentStatus(PurchaseStatus status, PaymentStatus paymentStatus)
        {
            var isPaidWorkflowStatus = status is PurchaseStatus.Paid
                or PurchaseStatus.Processing
                or PurchaseStatus.ReadyForPickup
                or PurchaseStatus.Shipped
                or PurchaseStatus.Completed;

            if (isPaidWorkflowStatus && paymentStatus != PaymentStatus.Succeeded)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }

            if (status == PurchaseStatus.PendingPayment && paymentStatus == PaymentStatus.Succeeded)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }

            if (status == PurchaseStatus.Refunded && paymentStatus != PaymentStatus.Refunded)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }

            if (status == PurchaseStatus.Cancelled && paymentStatus == PaymentStatus.Succeeded)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }

            if (status == PurchaseStatus.Failed && paymentStatus != PaymentStatus.Failed)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }
        }
    }
}
