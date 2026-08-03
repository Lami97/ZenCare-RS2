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

        public async Task<PagedResult<CartItemResponse>> GetMyAsync(int userId, CartItemSearchObject? search)
        {
            var query = GetOwnedCartItemQuery(userId);
            query = ApplyFilters(query, search);

            return await CreatePagedResultAsync(query, search);
        }

        public async Task<CartItemResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetOwnedCartItemQuery(userId)
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (entity == null)
            {
                throw new BusinessException("Cart item was not found.");
            }

            return Mapper.Map<CartItemResponse>(entity);
        }

        public async Task<CartItemResponse> CreateMyAsync(int userId, CartItemInsertRequest request)
        {
            if (request.Quantity <= 0)
            {
                throw new BusinessException("Quantity must be greater than zero.");
            }

            await EnsureCartBelongsToUserAsync(request.CartId, userId);

            var existingItem = await DbContext.CartItems
                .Include(ci => ci.Cart)
                    .ThenInclude(c => c.User)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == request.CartId && ci.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;

                await DbContext.SaveChangesAsync();

                return Mapper.Map<CartItemResponse>(existingItem);
            }

            return await InsertAsync(request);
        }

        public async Task<CartItemResponse> UpdateMyAsync(int id, int userId, CartItemUpdateRequest request)
        {
            await EnsureCartItemBelongsToUserAsync(id, userId);
            await EnsureCartBelongsToUserAsync(request.CartId, userId);

            request.Id = id;

            return await UpdateAsync(id, request);
        }

        public async Task DeleteMyAsync(int id, int userId)
        {
            await EnsureCartItemBelongsToUserAsync(id, userId);
            await DeleteAsync(id);
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

        private IQueryable<Database.CartItem> GetOwnedCartItemQuery(int userId)
        {
            return DbContext.CartItems
                .Include(ci => ci.Cart)
                    .ThenInclude(c => c.User)
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userId);
        }

        private async Task EnsureCartBelongsToUserAsync(int cartId, int userId)
        {
            var exists = await DbContext.Carts.AnyAsync(c => c.Id == cartId && c.UserId == userId);

            if (!exists)
            {
                throw new BusinessException("Cart was not found.");
            }
        }

        private async Task EnsureCartItemBelongsToUserAsync(int id, int userId)
        {
            var exists = await DbContext.CartItems.AnyAsync(ci => ci.Id == id && ci.Cart.UserId == userId);

            if (!exists)
            {
                throw new BusinessException("Cart item was not found.");
            }
        }

        private async Task<PagedResult<CartItemResponse>> CreatePagedResultAsync(IQueryable<Database.CartItem> query, CartItemSearchObject? search)
        {
            int? totalCount = null;

            if (search?.IncludeTotalCount == true)
            {
                totalCount = await query.CountAsync();
            }

            var entities = await query.ToListAsync();

            return new PagedResult<CartItemResponse>
            {
                Items = Mapper.Map<List<CartItemResponse>>(entities),
                TotalCount = totalCount
            };
        }
    }
}