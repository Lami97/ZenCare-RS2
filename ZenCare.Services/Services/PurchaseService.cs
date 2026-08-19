using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Text.Json;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Messages;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class PurchaseService : BaseCRUDService<PurchaseResponse, Database.Purchase, PurchaseInsertRequest, PurchaseUpdateRequest, PurchaseSearchObject>, IPurchaseService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly INotificationEventPublisher _notificationEventPublisher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            ZenCareDbContext dbContext,
            IMapper mapper,
            IRabbitMqService rabbitMqService,
            INotificationEventPublisher notificationEventPublisher,
            IConfiguration configuration,
            ILogger<PurchaseService> logger) : base(dbContext, mapper)
        {
            _rabbitMqService = rabbitMqService;
            _notificationEventPublisher = notificationEventPublisher;
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<PurchaseResponse> InsertAsync(PurchaseInsertRequest request)
        {
            ValidateGenericPurchaseInsert(request);
            ValidatePurchaseRequest(request.PurchaseNumber, request.TotalAmount, request.Status, request.PaymentStatus);

            return await base.InsertAsync(request);
        }

        public override async Task<PurchaseResponse> UpdateAsync(int id, PurchaseUpdateRequest request)
        {
            return await UpdateCoreAsync(id, request, null);
        }

        public async Task<PurchaseResponse> UpdateWithActorAsync(
            int id,
            int actorUserId,
            PurchaseUpdateRequest request)
        {
            return await UpdateCoreAsync(id, request, actorUserId);
        }

        private async Task<PurchaseResponse> UpdateCoreAsync(
            int id,
            PurchaseUpdateRequest request,
            int? actorUserId)
        {
            var entity = await DbContext.Purchases.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            var previousStatus = entity.Status;
            var previousPaymentStatus = entity.PaymentStatus;

            ValidateGenericPurchaseUpdate(entity, request);
            PreservePaymentEvidence(entity, request);
            ValidatePurchaseRequest(request.PurchaseNumber, request.TotalAmount, request.Status, request.PaymentStatus);
            ValidatePurchaseStatusTransition(entity.Status, request.Status);

            Mapper.Map(request, entity);
            SetUpdatedAt(entity);

            if (previousStatus != entity.Status || previousPaymentStatus != entity.PaymentStatus)
            {
                AddPurchaseStatusHistory(
                    entity,
                    previousStatus,
                    previousPaymentStatus,
                    actorUserId,
                    GetPurchaseStatusChangeDescription(entity.Status),
                    null);
            }

            await DbContext.SaveChangesAsync();
            var result = await GetByIdAsync(id);

            if (previousStatus != entity.Status && IsClientRelevantFulfillmentStatus(entity.Status))
            {
                await PublishPurchaseStatusChangedAsync(entity);
            }

            return result;
        }

        public async Task<PagedResult<PurchaseResponse>> GetMyAsync(int userId, PurchaseSearchObject? search)
        {
            search ??= new PurchaseSearchObject();
            search.UserId = userId;

            if (string.IsNullOrWhiteSpace(search.SortBy))
            {
                search.SortBy = "CreatedAt descending, Id descending";
            }

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
            var entity = await GetClientPurchaseEntityAsync(id, userId);

            if (request.Status != entity.Status)
            {
                throw new BusinessException("Purchase status can only be changed through the payment, cancellation, or fulfillment workflow.");
            }

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

            await PublishPurchaseCreatedMessageAsync(purchase);

            return await GetByIdAsync(purchase.Id);
        }

        public async Task<PurchaseResponse> CancelMyAsync(int id, int userId)
        {
            var purchase = await DbContext.Purchases
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (purchase == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            ValidatePurchaseForCancellation(purchase);

            var payment = await DbContext.Payments
                .FirstOrDefaultAsync(p => p.PurchaseId == purchase.Id && p.UserId == userId);

            ValidatePaymentForCancellation(payment);
            await CancelStripePaymentIntentIfNeededAsync(purchase, payment);

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var cancelledAt = DateTime.UtcNow;
            var previousStatus = purchase.Status;
            var previousPaymentStatus = purchase.PaymentStatus;

            foreach (var item in purchase.PurchaseItems)
            {
                item.Product.StockQuantity += item.Quantity;
                item.Product.UpdatedAt = cancelledAt;
            }

            purchase.Status = PurchaseStatus.Cancelled;
            purchase.PaymentStatus = PaymentStatus.Cancelled;
            purchase.UpdatedAt = cancelledAt;

            if (payment != null)
            {
                payment.Status = PaymentStatus.Cancelled;
                payment.UpdatedAt = cancelledAt;
            }

            AddPurchaseStatusHistory(
                purchase,
                previousStatus,
                previousPaymentStatus,
                userId,
                "Unpaid purchase cancelled by client.",
                null,
                cancelledAt);

            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(purchase.Id);
        }

        public async Task<List<PurchaseStatusHistoryResponse>> GetStatusHistoryAsync(int id)
        {
            if (!await DbContext.Purchases.AnyAsync(purchase => purchase.Id == id))
            {
                throw new NotFoundException(nameof(Database.Purchase), id);
            }

            return await DbContext.PurchaseStatusHistories
                .AsNoTracking()
                .Where(history => history.PurchaseId == id)
                .OrderBy(history => history.ChangedAt)
                .ThenBy(history => history.Id)
                .Select(history => new PurchaseStatusHistoryResponse
                {
                    Id = history.Id,
                    PurchaseId = history.PurchaseId,
                    OldStatus = history.OldStatus,
                    NewStatus = history.NewStatus,
                    OldPaymentStatus = history.OldPaymentStatus,
                    NewPaymentStatus = history.NewPaymentStatus,
                    ChangedByUserId = history.ChangedByUserId,
                    ChangedByUsername = history.ChangedByUser == null
                        ? null
                        : history.ChangedByUser.Username,
                    ChangedAt = history.ChangedAt,
                    Description = history.Description,
                    Reason = history.Reason
                })
                .ToListAsync();
        }

        private void AddPurchaseStatusHistory(
            Database.Purchase purchase,
            PurchaseStatus oldStatus,
            PaymentStatus oldPaymentStatus,
            int? actorUserId,
            string description,
            string? reason,
            DateTime? changedAt = null)
        {
            var purchaseStatusChanged = oldStatus != purchase.Status;
            var paymentStatusChanged = oldPaymentStatus != purchase.PaymentStatus;

            if (!purchaseStatusChanged && !paymentStatusChanged)
            {
                return;
            }

            DbContext.PurchaseStatusHistories.Add(new Database.PurchaseStatusHistory
            {
                PurchaseId = purchase.Id,
                OldStatus = oldStatus,
                NewStatus = purchase.Status,
                OldPaymentStatus = paymentStatusChanged ? oldPaymentStatus : null,
                NewPaymentStatus = paymentStatusChanged ? purchase.PaymentStatus : null,
                ChangedByUserId = actorUserId,
                ChangedAt = changedAt ?? purchase.UpdatedAt ?? DateTime.UtcNow,
                Description = description,
                Reason = reason
            });
        }

        private static string GetPurchaseStatusChangeDescription(PurchaseStatus status)
        {
            return status switch
            {
                PurchaseStatus.PendingPayment => "Purchase submitted for payment.",
                PurchaseStatus.Processing => "Purchase moved to processing.",
                PurchaseStatus.ReadyForPickup => "Purchase marked ready for pickup.",
                PurchaseStatus.Shipped => "Purchase marked shipped.",
                PurchaseStatus.Completed => "Purchase fulfilled.",
                PurchaseStatus.Cancelled => "Purchase cancelled by administrator.",
                PurchaseStatus.Failed => "Purchase marked failed.",
                _ => $"Purchase status changed to {status}."
            };
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

        private async Task PublishPurchaseCreatedMessageAsync(Database.Purchase purchase)
        {
            var message = new PurchaseCreatedMessage
            {
                PurchaseId = purchase.Id,
                PurchaseNumber = purchase.PurchaseNumber,
                UserId = purchase.UserId,
                TotalAmount = purchase.TotalAmount,
                CreatedAt = purchase.CreatedAt
            };

            try
            {
                var payload = JsonSerializer.Serialize(message);
                await _rabbitMqService.PublishAsync("purchase", payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Purchase {PurchaseId} was created, but the purchase event could not be published.", purchase.Id);
            }
        }

        private Task PublishPurchaseStatusChangedAsync(Database.Purchase purchase)
        {
            return _notificationEventPublisher.PublishAsync(new NotificationEventMessage
            {
                UserId = purchase.UserId,
                EventKey = $"purchase-status:{purchase.Id}:{(int)purchase.Status}",
                Title = "Purchase status updated",
                Message = $"Purchase {purchase.PurchaseNumber} status changed to {purchase.Status}.",
                OccurredAt = purchase.UpdatedAt ?? DateTime.UtcNow
            });
        }

        private static bool IsClientRelevantFulfillmentStatus(PurchaseStatus status)
        {
            return status is PurchaseStatus.Processing
                or PurchaseStatus.ReadyForPickup
                or PurchaseStatus.Shipped
                or PurchaseStatus.Completed;
        }

        private async Task CancelStripePaymentIntentIfNeededAsync(Database.Purchase purchase, Database.Payment? payment)
        {
            var paymentIntentId = purchase.StripePaymentIntentId ?? payment?.StripePaymentIntentId;

            if (string.IsNullOrWhiteSpace(paymentIntentId))
            {
                return;
            }

            var paymentIntentService = new PaymentIntentService();
            var requestOptions = new RequestOptions { ApiKey = GetStripeSecretKey() };

            PaymentIntent paymentIntent;

            try
            {
                paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, requestOptions: requestOptions);
            }
            catch (StripeException)
            {
                throw new BusinessException("Stripe payment intent could not be retrieved for cancellation.");
            }

            if (paymentIntent.Status == "canceled")
            {
                return;
            }

            if (paymentIntent.Status == "succeeded")
            {
                throw new BusinessException("Paid purchases cannot be cancelled.");
            }

            if (!IsCancellablePaymentIntent(paymentIntent))
            {
                throw new BusinessException("Stripe payment intent cannot be cancelled in its current state.");
            }

            try
            {
                await paymentIntentService.CancelAsync(paymentIntentId, requestOptions: requestOptions);
            }
            catch (StripeException)
            {
                throw new BusinessException("Stripe payment intent could not be cancelled.");
            }
        }

        private string GetStripeSecretKey()
        {
            var stripeSecretKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrWhiteSpace(stripeSecretKey))
            {
                throw new BusinessException("Stripe secret key is not configured.");
            }

            return stripeSecretKey;
        }

        private static bool IsCancellablePaymentIntent(PaymentIntent paymentIntent)
        {
            return paymentIntent.Status is "requires_payment_method"
                or "requires_confirmation"
                or "requires_action"
                or "requires_capture"
                or "processing";
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

        private static void ValidateGenericPurchaseInsert(PurchaseInsertRequest request)
        {
            if (request.Status is not PurchaseStatus.Draft and not PurchaseStatus.PendingPayment ||
                request.PaymentStatus != PaymentStatus.Pending)
            {
                throw new BusinessException("Purchases can only be created in Draft or PendingPayment status with Pending payment status.");
            }

            if (!string.IsNullOrWhiteSpace(request.StripePaymentIntentId) || request.PaidAt.HasValue)
            {
                throw new BusinessException("Stripe payment details can only be set through the payment workflow.");
            }
        }

        private static void ValidateGenericPurchaseUpdate(Database.Purchase entity, PurchaseUpdateRequest request)
        {
            if (entity.Status != request.Status && request.Status is PurchaseStatus.Paid or PurchaseStatus.Refunded)
            {
                throw new BusinessException("Paid and refunded statuses can only be set through the payment workflow.");
            }

            if (entity.PaymentStatus != request.PaymentStatus &&
                request.PaymentStatus is PaymentStatus.Succeeded or PaymentStatus.Refunded)
            {
                throw new BusinessException("Succeeded and refunded payment statuses can only be set through the payment workflow.");
            }

            if (request.TotalAmount != entity.TotalAmount)
            {
                throw new BusinessException("Purchase total can only be changed through purchase item operations.");
            }

            if (!string.IsNullOrWhiteSpace(request.StripePaymentIntentId) &&
                request.StripePaymentIntentId != entity.StripePaymentIntentId)
            {
                throw new BusinessException("Stripe payment details can only be set through the payment workflow.");
            }

            if (request.PaidAt.HasValue && request.PaidAt != entity.PaidAt)
            {
                throw new BusinessException("Payment date can only be set through the payment workflow.");
            }
        }

        private static void PreservePaymentEvidence(Database.Purchase entity, PurchaseUpdateRequest request)
        {
            request.UserId = entity.UserId;
            request.TotalAmount = entity.TotalAmount;
            request.StripePaymentIntentId = entity.StripePaymentIntentId;
            request.PaidAt = entity.PaidAt;
        }

        private static void ValidatePurchaseForCancellation(Database.Purchase purchase)
        {
            if (purchase.Status == PurchaseStatus.Paid || purchase.PaymentStatus == PaymentStatus.Succeeded)
            {
                throw new BusinessException("Paid purchases cannot be cancelled.");
            }

            if (purchase.Status == PurchaseStatus.Cancelled || purchase.PaymentStatus == PaymentStatus.Cancelled)
            {
                throw new BusinessException("This order has already been cancelled.");
            }

            if (purchase.Status == PurchaseStatus.Refunded || purchase.PaymentStatus == PaymentStatus.Refunded)
            {
                throw new BusinessException("Refunded purchases cannot be cancelled.");
            }

            if (purchase.Status != PurchaseStatus.PendingPayment || purchase.PaymentStatus != PaymentStatus.Pending)
            {
                throw new BusinessException("Only unpaid pending purchases can be cancelled.");
            }
        }

        private static void ValidatePaymentForCancellation(Database.Payment? payment)
        {
            if (payment == null)
            {
                return;
            }

            if (payment.Status == PaymentStatus.Succeeded)
            {
                throw new BusinessException("Paid purchases cannot be cancelled.");
            }

            if (payment.Status == PaymentStatus.Refunded)
            {
                throw new BusinessException("Refunded purchases cannot be cancelled.");
            }

            if (payment.Status == PaymentStatus.Cancelled)
            {
                throw new BusinessException("This order has already been cancelled.");
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                throw new BusinessException("Only unpaid pending purchases can be cancelled.");
            }
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
            var isValidCombination = status switch
            {
                PurchaseStatus.Draft => paymentStatus == PaymentStatus.Pending,
                PurchaseStatus.PendingPayment => paymentStatus == PaymentStatus.Pending,
                PurchaseStatus.Paid => paymentStatus == PaymentStatus.Succeeded,
                PurchaseStatus.Processing => paymentStatus == PaymentStatus.Succeeded,
                PurchaseStatus.ReadyForPickup => paymentStatus == PaymentStatus.Succeeded,
                PurchaseStatus.Shipped => paymentStatus == PaymentStatus.Succeeded,
                PurchaseStatus.Completed => paymentStatus == PaymentStatus.Succeeded,
                PurchaseStatus.Cancelled => paymentStatus == PaymentStatus.Cancelled,
                PurchaseStatus.Refunded => paymentStatus == PaymentStatus.Refunded,
                PurchaseStatus.Failed => paymentStatus == PaymentStatus.Failed,
                _ => false
            };

            if (!isValidCombination)
            {
                throw new BusinessException("Payment status is not valid for the selected purchase status.");
            }
        }
    }
}
