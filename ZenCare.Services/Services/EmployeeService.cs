using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class EmployeeService : BaseCRUDService<EmployeeResponse, Database.Employee, EmployeeInsertRequest, EmployeeUpdateRequest, EmployeeSearchObject>, IEmployeeService
    {
        public EmployeeService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<EmployeeResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Employee), id);
            }

            return Mapper.Map<EmployeeResponse>(entity);
        }

        protected override IQueryable<Database.Employee> ApplyFilters(IQueryable<Database.Employee> query, EmployeeSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(e => e.UserId == search.UserId.Value);
                }

                if (search.IsAvailable.HasValue)
                {
                    query = query.Where(e => e.IsAvailable == search.IsAvailable.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Employee>> IncludeRelatedEntitiesAsync(IQueryable<Database.Employee> query, EmployeeSearchObject? search)
        {
            query = query.Include(e => e.User);

            return Task.FromResult(query);
        }
    }
}
