using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public override async Task<PurchaseResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Purchases
                .Include(p => p.User)
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
            query = query.Include(p => p.User);

            return Task.FromResult(query);
        }

        private async Task<Database.Purchase> GetClientPurchaseEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Purchases
                .Include(p => p.User)
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
    }
}
