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
    public class ReviewService : BaseCRUDService<ReviewResponse, Database.Review, ReviewInsertRequest, ReviewUpdateRequest, ReviewSearchObject>, IReviewService
    {
        public ReviewService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ReviewResponse> InsertAsync(ReviewInsertRequest request)
        {
            await ValidateReviewCreateAsync(request.UserId, request.AppointmentId, request.ProductId, request.Rating, false);

            return await base.InsertAsync(request);
        }

        public override async Task<ReviewResponse> UpdateAsync(int id, ReviewUpdateRequest request)
        {
            var entity = await DbContext.Reviews.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            await ValidateReviewUpdateAsync(request.UserId, request.AppointmentId, request.ProductId, request.Rating, id);

            return await base.UpdateAsync(id, request);
        }

        public async Task<PagedResult<ReviewResponse>> GetMyAsync(int userId, ReviewSearchObject? search)
        {
            search ??= new ReviewSearchObject();
            search.UserId = userId;

            return await GetAllAsync(search);
        }

        public async Task<ReviewResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetClientReviewEntityAsync(id, userId);

            return Mapper.Map<ReviewResponse>(entity);
        }

        public async Task<ReviewResponse> InsertMyAsync(int userId, ReviewInsertRequest request)
        {
            request.UserId = userId;
            await ValidateReviewCreateAsync(userId, request.AppointmentId, request.ProductId, request.Rating, true);

            return await base.InsertAsync(request);
        }

        public async Task<ReviewResponse> UpdateMyAsync(int id, int userId, ReviewUpdateRequest request)
        {
            var existingReview = await GetClientReviewEntityAsync(id, userId);

            request.Id = id;
            request.UserId = userId;

            await ValidateClientReviewUpdateAsync(userId, existingReview, request.AppointmentId, request.ProductId, request.Rating);

            return await base.UpdateAsync(id, request);
        }

        public async Task DeleteMyAsync(int id, int userId)
        {
            var entity = await DbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            DbContext.Reviews.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task<ReviewResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            return Mapper.Map<ReviewResponse>(entity);
        }

        protected override IQueryable<Database.Review> ApplyFilters(IQueryable<Database.Review> query, ReviewSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(r => r.UserId == search.UserId.Value);
                }

                if (search.AppointmentId.HasValue)
                {
                    query = query.Where(r => r.AppointmentId == search.AppointmentId.Value);
                }

                if (search.ProductId.HasValue)
                {
                    query = query.Where(r => r.ProductId == search.ProductId.Value);
                }

                if (search.Rating.HasValue)
                {
                    query = query.Where(r => r.Rating == search.Rating.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(r => r.Status == search.Status.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Review>> IncludeRelatedEntitiesAsync(IQueryable<Database.Review> query, ReviewSearchObject? search)
        {
            query = query
                .Include(r => r.User)
                .Include(r => r.Product);

            return Task.FromResult(query);
        }

        private async Task ValidateReviewCreateAsync(int userId, int? appointmentId, int? productId, int rating, bool enforceClientOwnershipAndPurchase)
        {
            ValidateReviewRequest(appointmentId, productId, rating);

            if (appointmentId.HasValue)
            {
                await ValidateAppointmentReviewCreateAsync(userId, appointmentId.Value, null);
                return;
            }

            if (enforceClientOwnershipAndPurchase)
            {
                await ValidateProductReviewCreateAsync(userId, productId!.Value);
            }
        }

        private async Task ValidateClientReviewUpdateAsync(int userId, Database.Review existingReview, int? appointmentId, int? productId, int rating)
        {
            ValidateReviewRequest(appointmentId, productId, rating);

            if (existingReview.AppointmentId != appointmentId || existingReview.ProductId != productId)
            {
                throw new BusinessException("Review target cannot be changed.");
            }

            if (appointmentId.HasValue)
            {
                await ValidateAppointmentReviewEligibilityAsync(userId, appointmentId.Value);
                return;
            }

            await ValidateProductReviewEligibilityAsync(userId, productId!.Value);
        }

        private async Task ValidateReviewUpdateAsync(int userId, int? appointmentId, int? productId, int rating, int currentReviewId)
        {
            ValidateReviewRequest(appointmentId, productId, rating);

            if (appointmentId.HasValue)
            {
                await ValidateAppointmentReviewCreateAsync(userId, appointmentId.Value, currentReviewId);
            }
        }

        private async Task ValidateAppointmentReviewCreateAsync(int userId, int appointmentId, int? currentReviewId)
        {
            await ValidateAppointmentReviewEligibilityAsync(userId, appointmentId);

            var alreadyReviewed = await DbContext.Reviews
                .AnyAsync(r => r.AppointmentId == appointmentId
                    && (!currentReviewId.HasValue || r.Id != currentReviewId.Value));

            if (alreadyReviewed)
            {
                throw new BusinessException("This appointment has already been reviewed.");
            }
        }

        private async Task ValidateAppointmentReviewEligibilityAsync(int userId, int appointmentId)
        {
            var appointment = await DbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                throw new BusinessException("Only completed appointments can be reviewed.");
            }

            if (appointment.UserId != userId)
            {
                throw new BusinessException("The selected appointment does not belong to the selected user.");
            }

            if (appointment.Status != AppointmentStatus.Completed)
            {
                throw new BusinessException("Only completed appointments can be reviewed.");
            }

        }

        private async Task ValidateProductReviewCreateAsync(int userId, int productId)
        {
            await ValidateProductReviewEligibilityAsync(userId, productId);

            var alreadyReviewed = await DbContext.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

            if (alreadyReviewed)
            {
                throw new BusinessException("You have already reviewed this product.");
            }
        }

        private async Task ValidateProductReviewEligibilityAsync(int userId, int productId)
        {
            var productExists = await DbContext.Products
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                throw new BusinessException("You can review only products you have purchased.");
            }

            var hasPurchasedProduct = await DbContext.PurchaseItems
                .AnyAsync(pi => pi.ProductId == productId && pi.Purchase.UserId == userId);

            if (!hasPurchasedProduct)
            {
                throw new BusinessException("You can review only products you have purchased.");
            }

            var hasCompletedPaidPurchase = await DbContext.PurchaseItems
                .AnyAsync(pi =>
                    pi.ProductId == productId &&
                    pi.Purchase.UserId == userId &&
                    pi.Purchase.Status == PurchaseStatus.Completed &&
                    pi.Purchase.PaymentStatus == PaymentStatus.Succeeded);

            if (!hasCompletedPaidPurchase)
            {
                throw new BusinessException("The purchase must be completed and paid before the product can be reviewed.");
            }
        }

        private static void ValidateReviewRequest(int? appointmentId, int? productId, int rating)
        {
            if (appointmentId.HasValue == productId.HasValue)
            {
                throw new BusinessException("Select exactly one review target.");
            }

            if (rating < 1 || rating > 5)
            {
                throw new BusinessException("Rating must be between 1 and 5.");
            }
        }

        private async Task<Database.Review> GetClientReviewEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }

            return entity;
        }

        private async Task EnsureClientReviewExistsAsync(int id, int userId)
        {
            var exists = await DbContext.Reviews
                .AnyAsync(r => r.Id == id && r.UserId == userId);

            if (!exists)
            {
                throw new NotFoundException(nameof(Database.Review), id);
            }
        }
    }
}
