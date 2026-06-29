using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class UserRoleService : BaseCRUDService<UserRoleResponse, Database.UserRole, UserRoleInsertRequest, UserRoleUpdateRequest, UserRoleSearchObject>, IUserRoleService
    {
        public UserRoleService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<UserRoleResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.UserRole), id);
            }

            return Mapper.Map<UserRoleResponse>(entity);
        }

        protected override IQueryable<Database.UserRole> ApplyFilters(IQueryable<Database.UserRole> query, UserRoleSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(ur => ur.UserId == search.UserId.Value);
                }

                if (search.RoleId.HasValue)
                {
                    query = query.Where(ur => ur.RoleId == search.RoleId.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.UserRole>> IncludeRelatedEntitiesAsync(IQueryable<Database.UserRole> query, UserRoleSearchObject? search)
        {
            query = query
                .Include(ur => ur.User)
                .Include(ur => ur.Role);

            return Task.FromResult(query);
        }
    }
}
