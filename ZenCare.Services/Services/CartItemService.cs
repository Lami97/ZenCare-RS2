using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class CartItemService : BaseCRUDService<CartItemResponse, Database.CartItem, CartItemInsertRequest, CartItemUpdateRequest, CartItemSearchObject>, ICartItemService
    {
        public CartItemService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<CartItemResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.CartItems
                .Include(ci => ci.Cart)
                    .ThenInclude(c => c.User)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.CartItem), id);
            }

            return Mapper.Map<CartItemResponse>(entity);
        }

        protected override IQueryable<Database.CartItem> ApplyFilters(IQueryable<Database.CartItem> query, CartItemSearchObject? search)
        {
            if (search != null)
            {
                if (search.CartId.HasValue)
                {
                    query = query.Where(ci => ci.CartId == search.CartId.Value);
                }

                if (search.ProductId.HasValue)
                {
                    query = query.Where(ci => ci.ProductId == search.ProductId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.CartItem>> IncludeRelatedEntitiesAsync(IQueryable<Database.CartItem> query, CartItemSearchObject? search)
        {
            query = query
                .Include(ci => ci.Cart)
                    .ThenInclude(c => c.User)
                .Include(ci => ci.Product);

            return Task.FromResult(query);
        }
    }
}
