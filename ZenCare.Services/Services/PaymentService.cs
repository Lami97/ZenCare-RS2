using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class PaymentService : BaseCRUDService<PaymentResponse, Database.Payment, PaymentInsertRequest, PaymentUpdateRequest, PaymentSearchObject>, IPaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(ZenCareDbContext dbContext, IMapper mapper, IConfiguration configuration) : base(dbContext, mapper)
        {
            _configuration = configuration;
        }

        public override Task<PaymentResponse> InsertAsync(PaymentInsertRequest request)
        {
            throw new BusinessException("Payment records can only be created through the payment workflow.");
        }

        public override Task<PaymentResponse> UpdateAsync(int id, PaymentUpdateRequest request)
        {
            throw new BusinessException("Payment records can only be updated through the payment workflow.");
        }

        public override Task DeleteAsync(int id)
        {
            throw new BusinessException("Payment records cannot be deleted.");
        }

        public async Task<PagedResult<PaymentResponse>> GetMyAsync(int userId, PaymentSearchObject? search)
        {
            var query = DbContext.Payments
                .Include(p => p.User)
                .Where(p => p.UserId == userId);

            query = ApplyFilters(query, search);

            return await CreatePagedResultAsync(query, search);
        }

        public async Task<PaymentResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await DbContext.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (entity == null)
            {
                throw new BusinessException("Payment was not found.");
            }

            return Mapper.Map<PaymentResponse>(entity);
        }

        public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(int purchaseId, int userId)
        {
            var stripeSecretKey = GetStripeSecretKey();
            var currency = GetStripeCurrency();
            var requestOptions = new RequestOptions { ApiKey = stripeSecretKey };
            var paymentIntentService = new PaymentIntentService();

            var purchase = await DbContext.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            ValidatePurchaseForIntent(purchase, purchaseId, userId);

            var existingPayment = await DbContext.Payments
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId && p.UserId == userId);

            if (!string.IsNullOrWhiteSpace(purchase!.StripePaymentIntentId))
            {
                var existingIntent = await GetExistingPaymentIntentAsync(paymentIntentService, purchase.StripePaymentIntentId, requestOptions);

                if (!IsUsablePaymentIntent(existingIntent))
                {
                    throw new BusinessException("Existing payment intent cannot be reused for this purchase.");
                }

                if (existingPayment == null)
                {
                    existingPayment = await CreateLocalPaymentAsync(purchase, userId, currency, existingIntent.Id);
                }

                return CreateResponse(purchase, existingPayment, existingIntent);
            }

            if (existingPayment != null && !string.IsNullOrWhiteSpace(existingPayment.StripePaymentIntentId))
            {
                var existingIntent = await GetExistingPaymentIntentAsync(paymentIntentService, existingPayment.StripePaymentIntentId, requestOptions);

                if (!IsUsablePaymentIntent(existingIntent))
                {
                    throw new BusinessException("Existing payment intent cannot be reused for this purchase.");
                }

                await using var existingTransaction = await DbContext.Database.BeginTransactionAsync();

                purchase.StripePaymentIntentId = existingIntent.Id;
                purchase.UpdatedAt = DateTime.UtcNow;
                await DbContext.SaveChangesAsync();
                await existingTransaction.CommitAsync();

                return CreateResponse(purchase, existingPayment, existingIntent);
            }

            var createOptions = new PaymentIntentCreateOptions
            {
                Amount = ConvertAmountToSmallestCurrencyUnit(purchase.TotalAmount),
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "PurchaseId", purchase.Id.ToString() },
                    { "UserId", userId.ToString() },
                    { "PurchaseNumber", purchase.PurchaseNumber }
                }
            };

            PaymentIntent paymentIntent;

            try
            {
                paymentIntent = await paymentIntentService.CreateAsync(
                    createOptions,
                    new RequestOptions
                    {
                        ApiKey = stripeSecretKey,
                        IdempotencyKey = $"purchase-{purchase.Id}-payment-intent"
                    });
            }
            catch (StripeException)
            {
                throw new BusinessException("Stripe payment intent could not be created.");
            }

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var payment = new Database.Payment
            {
                UserId = userId,
                PurchaseId = purchase.Id,
                Amount = purchase.TotalAmount,
                Currency = currency,
                Status = PaymentStatus.Pending,
                StripePaymentIntentId = paymentIntent.Id,
                CreatedAt = DateTime.UtcNow
            };

            purchase.StripePaymentIntentId = paymentIntent.Id;
            purchase.UpdatedAt = DateTime.UtcNow;

            DbContext.Payments.Add(payment);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreateResponse(purchase, payment, paymentIntent);
        }

        public async Task<PaymentConfirmResponse> ConfirmPaymentAsync(int purchaseId, int userId)
        {
            var stripeSecretKey = GetStripeSecretKey();
            var requestOptions = new RequestOptions { ApiKey = stripeSecretKey };
            var paymentIntentService = new PaymentIntentService();

            var purchase = await DbContext.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            var purchaseEntity = ValidatePurchaseOwnership(purchase, purchaseId, userId);

            if (purchaseEntity.Status == PurchaseStatus.Paid &&
                purchaseEntity.PaymentStatus == PaymentStatus.Succeeded)
            {
                return await ConfirmAlreadySuccessfulPaymentAsync(
                    purchaseEntity,
                    userId,
                    paymentIntentService,
                    requestOptions);
            }

            ValidatePurchaseForConfirmation(purchaseEntity);

            var payment = await DbContext.Payments
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId && p.UserId == userId);

            if (payment == null)
            {
                throw new BusinessException("Payment record was not found for this purchase.");
            }

            var paymentIntent = await GetPaymentIntentForConfirmationAsync(
                paymentIntentService,
                purchaseEntity.StripePaymentIntentId!,
                requestOptions);

            if (IsIncompletePaymentIntent(paymentIntent))
            {
                throw new BusinessException("Payment is not yet complete.");
            }

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            if (paymentIntent.Status == "succeeded")
            {
                var paidAt = DateTime.UtcNow;

                purchaseEntity.Status = PurchaseStatus.Paid;
                purchaseEntity.PaymentStatus = PaymentStatus.Succeeded;
                purchaseEntity.PaidAt = paidAt;
                purchaseEntity.UpdatedAt = paidAt;

                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = paidAt;
                payment.UpdatedAt = paidAt;
                payment.StripeChargeId = paymentIntent.LatestChargeId;
            }
            else if (paymentIntent.Status == "canceled")
            {
                purchaseEntity.Status = PurchaseStatus.Cancelled;
                purchaseEntity.PaymentStatus = PaymentStatus.Cancelled;
                purchaseEntity.UpdatedAt = DateTime.UtcNow;

                payment.Status = PaymentStatus.Cancelled;
                payment.UpdatedAt = DateTime.UtcNow;
            }
            else if (paymentIntent.Status == "failed")
            {
                purchaseEntity.Status = PurchaseStatus.Failed;
                purchaseEntity.PaymentStatus = PaymentStatus.Failed;
                purchaseEntity.UpdatedAt = DateTime.UtcNow;

                payment.Status = PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new BusinessException("Stripe payment status cannot be confirmed yet.");
            }

            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreateConfirmResponse(purchaseEntity, payment);
        }

        public async Task<PaymentRefundResponse> RefundPaymentAsync(int purchaseId, int userId)
        {
            var stripeSecretKey = GetStripeSecretKey();
            var requestOptions = new RequestOptions { ApiKey = stripeSecretKey };
            var paymentIntentService = new PaymentIntentService();
            var refundService = new RefundService();

            var purchase = await DbContext.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            ValidatePurchaseForRefund(purchase, purchaseId, userId);
            var purchaseEntity = purchase!;

            var payment = await DbContext.Payments
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId && p.UserId == userId);

            ValidatePaymentForRefund(payment);
            var paymentEntity = payment!;

            var paymentIntent = await GetPaymentIntentForConfirmationAsync(
                paymentIntentService,
                paymentEntity.StripePaymentIntentId!,
                requestOptions);

            var chargeId = paymentEntity.StripeChargeId ?? paymentIntent.LatestChargeId;

            if (string.IsNullOrWhiteSpace(chargeId))
            {
                throw new BusinessException("Payment charge was not found for this purchase.");
            }

            Refund refund;

            try
            {
                refund = await refundService.CreateAsync(
                    new RefundCreateOptions
                    {
                        Charge = chargeId
                    },
                    requestOptions);
            }
            catch (StripeException)
            {
                throw new BusinessException("Stripe refund could not be created.");
            }

            if (refund.Status is "failed" or "canceled")
            {
                throw new BusinessException("Stripe refund was not successful.");
            }

            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var refundedAt = DateTime.UtcNow;

            purchaseEntity.Status = PurchaseStatus.Refunded;
            purchaseEntity.PaymentStatus = PaymentStatus.Refunded;
            purchaseEntity.UpdatedAt = refundedAt;

            paymentEntity.Status = PaymentStatus.Refunded;
            paymentEntity.UpdatedAt = refundedAt;

            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new PaymentRefundResponse
            {
                PurchaseId = purchaseEntity.Id,
                PaymentId = paymentEntity.Id,
                PurchaseStatus = purchaseEntity.Status,
                PaymentStatus = purchaseEntity.PaymentStatus,
                RefundedAt = refundedAt
            };
        }

        public override async Task<PaymentResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Payment), id);
            }

            return Mapper.Map<PaymentResponse>(entity);
        }

        protected override IQueryable<Database.Payment> ApplyFilters(IQueryable<Database.Payment> query, PaymentSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(p => p.UserId == search.UserId.Value);
                }

                if (search.AppointmentId.HasValue)
                {
                    query = query.Where(p => p.AppointmentId == search.AppointmentId.Value);
                }

                if (search.PurchaseId.HasValue)
                {
                    query = query.Where(p => p.PurchaseId == search.PurchaseId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(p => p.Status == search.Status.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Payment>> IncludeRelatedEntitiesAsync(IQueryable<Database.Payment> query, PaymentSearchObject? search)
        {
            query = query.Include(p => p.User);

            return Task.FromResult(query);
        }

        private async Task<PagedResult<PaymentResponse>> CreatePagedResultAsync(IQueryable<Database.Payment> query, PaymentSearchObject? search)
        {
            int? totalCount = null;

            if (search?.IncludeTotalCount == true)
            {
                totalCount = await query.CountAsync();
            }

            query = query.OrderBy(p => p.Id);
            query = ApplyPagination(query, search);

            var entities = await query.ToListAsync();

            return new PagedResult<PaymentResponse>
            {
                Items = Mapper.Map<List<PaymentResponse>>(entities),
                TotalCount = totalCount
            };
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

        private string GetStripeCurrency()
        {
            var currency = _configuration["Stripe:Currency"];

            return string.IsNullOrWhiteSpace(currency) ? "usd" : currency.Trim().ToLowerInvariant();
        }

        private static void ValidatePurchaseForIntent(Database.Purchase? purchase, int purchaseId, int userId)
        {
            if (purchase == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            if (purchase.UserId != userId)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            if (purchase.Status != PurchaseStatus.PendingPayment || purchase.PaymentStatus != PaymentStatus.Pending)
            {
                throw new BusinessException("Payment intent can only be created for a pending purchase.");
            }

            if (purchase.TotalAmount <= 0)
            {
                throw new BusinessException("Purchase amount must be greater than zero.");
            }

            if (purchase.PurchaseItems.Count == 0)
            {
                throw new BusinessException("Purchase must contain at least one item.");
            }
        }

        private static long ConvertAmountToSmallestCurrencyUnit(decimal amount)
        {
            return decimal.ToInt64(decimal.Round(amount * 100m, 0, MidpointRounding.AwayFromZero));
        }

        private static bool IsUsablePaymentIntent(PaymentIntent paymentIntent)
        {
            return paymentIntent.Status is "requires_payment_method"
                or "requires_confirmation"
                or "requires_action"
                or "processing";
        }

        private async Task<PaymentIntent> GetExistingPaymentIntentAsync(
            PaymentIntentService paymentIntentService,
            string paymentIntentId,
            RequestOptions requestOptions)
        {
            try
            {
                return await paymentIntentService.GetAsync(paymentIntentId, requestOptions: requestOptions);
            }
            catch (StripeException)
            {
                throw new BusinessException("Existing payment intent could not be retrieved from Stripe.");
            }
        }

        private async Task<PaymentIntent> GetPaymentIntentForConfirmationAsync(
            PaymentIntentService paymentIntentService,
            string paymentIntentId,
            RequestOptions requestOptions)
        {
            try
            {
                return await paymentIntentService.GetAsync(paymentIntentId, requestOptions: requestOptions);
            }
            catch (StripeException)
            {
                throw new BusinessException("Stripe payment status could not be retrieved.");
            }
        }

        private static Database.Purchase ValidatePurchaseOwnership(Database.Purchase? purchase, int purchaseId, int userId)
        {
            if (purchase == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            if (purchase.UserId != userId)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            return purchase;
        }

        private static void ValidatePurchaseForConfirmation(Database.Purchase purchase)
        {
            if (purchase.Status != PurchaseStatus.PendingPayment || purchase.PaymentStatus != PaymentStatus.Pending)
            {
                throw new BusinessException("Payment can only be confirmed for a pending purchase.");
            }

            if (string.IsNullOrWhiteSpace(purchase.StripePaymentIntentId))
            {
                throw new BusinessException("Stripe payment intent was not created for this purchase.");
            }
        }

        private async Task<PaymentConfirmResponse> ConfirmAlreadySuccessfulPaymentAsync(
            Database.Purchase purchase,
            int userId,
            PaymentIntentService paymentIntentService,
            RequestOptions requestOptions)
        {
            var payment = await DbContext.Payments
                .FirstOrDefaultAsync(p => p.PurchaseId == purchase.Id && p.UserId == userId);

            var expectedCurrency = GetStripeCurrency();
            ValidateAlreadySuccessfulPayment(purchase, payment, expectedCurrency);

            var paymentIntent = await GetPaymentIntentForConfirmationAsync(
                paymentIntentService,
                purchase.StripePaymentIntentId!,
                requestOptions);

            ValidateAlreadySuccessfulPaymentIntent(purchase, payment!, paymentIntent, expectedCurrency);

            return CreateConfirmResponse(purchase, payment!);
        }

        private static void ValidateAlreadySuccessfulPayment(
            Database.Purchase purchase,
            Database.Payment? payment,
            string expectedCurrency)
        {
            if (payment == null)
            {
                throw new BusinessException("Payment record was not found for this purchase.");
            }

            if (payment.Status != PaymentStatus.Succeeded)
            {
                throw new BusinessException("Payment state is inconsistent with the paid purchase.");
            }

            if (string.IsNullOrWhiteSpace(purchase.StripePaymentIntentId) ||
                string.IsNullOrWhiteSpace(payment.StripePaymentIntentId) ||
                !string.Equals(purchase.StripePaymentIntentId, payment.StripePaymentIntentId, StringComparison.Ordinal))
            {
                throw new BusinessException("Stripe payment evidence is missing or inconsistent.");
            }

            if (payment.Amount != purchase.TotalAmount)
            {
                throw new BusinessException("Payment amount does not match the purchase total.");
            }

            if (!string.Equals(payment.Currency, expectedCurrency, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("Payment currency is not valid for this purchase.");
            }

            if (!purchase.PaidAt.HasValue || !payment.PaidAt.HasValue || purchase.PaidAt != payment.PaidAt)
            {
                throw new BusinessException("Payment completion time is missing or inconsistent.");
            }
        }

        private static void ValidateAlreadySuccessfulPaymentIntent(
            Database.Purchase purchase,
            Database.Payment payment,
            PaymentIntent paymentIntent,
            string expectedCurrency)
        {
            if (!string.Equals(paymentIntent.Id, purchase.StripePaymentIntentId, StringComparison.Ordinal) ||
                !string.Equals(paymentIntent.Id, payment.StripePaymentIntentId, StringComparison.Ordinal))
            {
                throw new BusinessException("Stripe payment intent does not match this purchase.");
            }

            if (paymentIntent.Status != "succeeded")
            {
                throw new BusinessException("Stripe payment is not in a succeeded state.");
            }

            if (paymentIntent.Amount != ConvertAmountToSmallestCurrencyUnit(purchase.TotalAmount))
            {
                throw new BusinessException("Stripe payment amount does not match the purchase total.");
            }

            if (!string.Equals(paymentIntent.Currency, expectedCurrency, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("Stripe payment currency does not match this purchase.");
            }
        }

        private static bool IsIncompletePaymentIntent(PaymentIntent paymentIntent)
        {
            return paymentIntent.Status is "requires_payment_method"
                or "requires_confirmation"
                or "requires_action"
                or "processing"
                or "requires_capture";
        }

        private static void ValidatePurchaseForRefund(Database.Purchase? purchase, int purchaseId, int userId)
        {
            if (purchase == null)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            if (purchase.UserId != userId)
            {
                throw new NotFoundException(nameof(Database.Purchase), purchaseId);
            }

            if (purchase.Status == PurchaseStatus.Refunded || purchase.PaymentStatus == PaymentStatus.Refunded)
            {
                throw new BusinessException("This payment has already been refunded.");
            }

            if (purchase.Status != PurchaseStatus.Paid || purchase.PaymentStatus != PaymentStatus.Succeeded)
            {
                throw new BusinessException("Only succeeded paid purchases can be refunded.");
            }
        }

        private static void ValidatePaymentForRefund(Database.Payment? payment)
        {
            if (payment == null)
            {
                throw new BusinessException("Payment record was not found for this purchase.");
            }

            if (payment.Status == PaymentStatus.Refunded)
            {
                throw new BusinessException("This payment has already been refunded.");
            }

            if (payment.Status != PaymentStatus.Succeeded)
            {
                throw new BusinessException("Only succeeded payments can be refunded.");
            }

            if (string.IsNullOrWhiteSpace(payment.StripePaymentIntentId))
            {
                throw new BusinessException("Stripe payment intent was not found for this payment.");
            }
        }

        private async Task<Database.Payment> CreateLocalPaymentAsync(
            Database.Purchase purchase,
            int userId,
            string currency,
            string paymentIntentId)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var payment = new Database.Payment
            {
                UserId = userId,
                PurchaseId = purchase.Id,
                Amount = purchase.TotalAmount,
                Currency = currency,
                Status = PaymentStatus.Pending,
                StripePaymentIntentId = paymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.Payments.Add(payment);
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return payment;
        }

        private static PaymentIntentResponse CreateResponse(
            Database.Purchase purchase,
            Database.Payment payment,
            PaymentIntent paymentIntent)
        {
            return new PaymentIntentResponse
            {
                PurchaseId = purchase.Id,
                PaymentId = payment.Id,
                StripePaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = purchase.TotalAmount,
                Currency = payment.Currency,
                Status = payment.Status
            };
        }

        private static PaymentConfirmResponse CreateConfirmResponse(
            Database.Purchase purchase,
            Database.Payment payment)
        {
            return new PaymentConfirmResponse
            {
                PurchaseId = purchase.Id,
                PaymentId = payment.Id,
                PurchaseStatus = purchase.Status,
                PaymentStatus = purchase.PaymentStatus,
                PaidAt = purchase.PaidAt
            };
        }
    }
}
