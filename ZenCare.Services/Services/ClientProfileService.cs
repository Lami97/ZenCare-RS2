using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ClientProfileService : BaseCRUDService<ClientProfileResponse, Database.ClientProfile, ClientProfileInsertRequest, ClientProfileUpdateRequest, ClientProfileSearchObject>, IClientProfileService
    {
        public ClientProfileService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ClientProfileResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.ClientProfiles
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.ClientProfile), id);
            }

            return Mapper.Map<ClientProfileResponse>(entity);
        }

        protected override IQueryable<Database.ClientProfile> ApplyFilters(IQueryable<Database.ClientProfile> query, ClientProfileSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(cp => cp.UserId == search.UserId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.ClientProfile>> IncludeRelatedEntitiesAsync(IQueryable<Database.ClientProfile> query, ClientProfileSearchObject? search)
        {
            query = query.Include(cp => cp.User);

            return Task.FromResult(query);
        }
    }
}
