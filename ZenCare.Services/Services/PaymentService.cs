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
    }
}
