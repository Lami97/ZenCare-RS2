using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    }
}
