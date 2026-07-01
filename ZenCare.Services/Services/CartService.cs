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
    }
}
