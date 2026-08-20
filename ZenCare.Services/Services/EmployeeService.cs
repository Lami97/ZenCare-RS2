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
        private readonly TimeZoneInfo _businessTimeZone;

        public EmployeeService(
            ZenCareDbContext dbContext,
            IMapper mapper,
            TimeZoneInfo businessTimeZone) : base(dbContext, mapper)
        {
            _businessTimeZone = businessTimeZone;
        }

        public override Task<EmployeeResponse> InsertAsync(EmployeeInsertRequest request)
        {
            ValidateHireDate(request.HireDate);
            return base.InsertAsync(request);
        }

        public override Task<EmployeeResponse> UpdateAsync(int id, EmployeeUpdateRequest request)
        {
            ValidateHireDate(request.HireDate);
            return base.UpdateAsync(id, request);
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
                if (!string.IsNullOrWhiteSpace(search.SearchTerm))
                {
                    var searchTerm = search.SearchTerm.Trim();
                    query = query.Where(e =>
                        e.User.FirstName.Contains(searchTerm) ||
                        e.User.LastName.Contains(searchTerm) ||
                        (e.User.FirstName + " " + e.User.LastName).Contains(searchTerm) ||
                        e.User.Username.Contains(searchTerm) ||
                        (e.Specialization != null && e.Specialization.Contains(searchTerm)));
                }

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

        private void ValidateHireDate(DateTime? hireDate)
        {
            var businessToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _businessTimeZone).Date;

            if (hireDate.HasValue && hireDate.Value.Date > businessToday)
            {
                throw new BusinessException("Hire date cannot be in the future.");
            }
        }
    }
}
