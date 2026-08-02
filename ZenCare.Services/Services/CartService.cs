using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class CartService : BaseCRUDService<CartResponse, Database.Cart, CartInsertRequest, CartUpdateRequest, CartSearchObject>, ICartService
    {
        public CartService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<CartResponse> GetMyAsync(int userId)
        {
            var entity = await DbContext.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (entity == null)
            {
                throw new BusinessException("Cart was not found.");
            }

            return Mapper.Map<CartResponse>(entity);
        }

        public async Task<CartResponse> CreateMyAsync(int userId, CartInsertRequest request)
        {
            request.UserId = userId;
            return await InsertAsync(request);
        }

        public async Task<CartResponse> UpdateMyAsync(int id, int userId, CartUpdateRequest request)
        {
            await EnsureCartBelongsToUserAsync(id, userId);

            request.Id = id;
            request.UserId = userId;

            return await UpdateAsync(id, request);
        }

        public async Task DeleteMyAsync(int id, int userId)
        {
            await EnsureCartBelongsToUserAsync(id, userId);
            await DeleteAsync(id);
        }

        public override async Task<CartResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Cart), id);
            }

            return Mapper.Map<CartResponse>(entity);
        }

        protected override IQueryable<Database.Cart> ApplyFilters(IQueryable<Database.Cart> query, CartSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(c => c.UserId == search.UserId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Cart>> IncludeRelatedEntitiesAsync(IQueryable<Database.Cart> query, CartSearchObject? search)
        {
            query = query.Include(c => c.User);

            return Task.FromResult(query);
        }

        private async Task EnsureCartBelongsToUserAsync(int id, int userId)
        {
            var exists = await DbContext.Carts.AnyAsync(c => c.Id == id && c.UserId == userId);

            if (!exists)
            {
                throw new BusinessException("Cart was not found.");
            }
        }
    }
}