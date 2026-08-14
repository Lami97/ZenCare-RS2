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

        public override async Task<UserRoleResponse> InsertAsync(UserRoleInsertRequest request)
        {
            await EnsureAssignmentIsUniqueAsync(request.UserId, request.RoleId);

            return await base.InsertAsync(request);
        }

        public override async Task<UserRoleResponse> UpdateAsync(int id, UserRoleUpdateRequest request)
        {
            await EnsureAssignmentIsUniqueAsync(request.UserId, request.RoleId, id);

            return await base.UpdateAsync(id, request);
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
                if (!string.IsNullOrWhiteSpace(search.Username))
                {
                    var username = search.Username.Trim();
                    query = query.Where(ur => ur.User.Username.Contains(username));
                }

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

        private async Task EnsureAssignmentIsUniqueAsync(int userId, int roleId, int? excludedId = null)
        {
            var assignmentExists = await DbContext.UserRoles.AnyAsync(userRole =>
                userRole.UserId == userId &&
                userRole.RoleId == roleId &&
                (!excludedId.HasValue || userRole.Id != excludedId.Value));

            if (assignmentExists)
            {
                throw new BusinessException("This user already has the selected role.");
            }
        }
    }
}
