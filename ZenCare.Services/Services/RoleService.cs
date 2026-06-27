using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class RoleService : BaseCRUDService<RoleResponse, Database.Role, RoleInsertRequest, RoleUpdateRequest, RoleSearchObject>, IRoleService
    {
        public RoleService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.Role> ApplyFilters(IQueryable<Database.Role> query, RoleSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(r => r.Name.Contains(search.Name));
                }

                if (search.RoleType.HasValue)
                {
                    query = query.Where(r => r.RoleType == search.RoleType.Value);
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(r => r.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
