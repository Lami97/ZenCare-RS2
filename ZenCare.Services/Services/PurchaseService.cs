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
    }
}
