using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class EmployeeServiceService : BaseCRUDService<EmployeeServiceResponse, Database.EmployeeService, EmployeeServiceInsertRequest, EmployeeServiceUpdateRequest, EmployeeServiceSearchObject>, IEmployeeServiceService
    {
        public EmployeeServiceService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<EmployeeServiceResponse> InsertAsync(EmployeeServiceInsertRequest request)
        {
            await EnsureAssignmentIsUniqueAsync(request.EmployeeId, request.WellnessServiceId);

            return await base.InsertAsync(request);
        }

        public override async Task<EmployeeServiceResponse> UpdateAsync(int id, EmployeeServiceUpdateRequest request)
        {
            await EnsureAssignmentIsUniqueAsync(request.EmployeeId, request.WellnessServiceId, id);

            return await base.UpdateAsync(id, request);
        }

        public override async Task<EmployeeServiceResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.EmployeeServices
                .Include(es => es.Employee)
                    .ThenInclude(e => e.User)
                .Include(es => es.WellnessService)
                .FirstOrDefaultAsync(es => es.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.EmployeeService), id);
            }

            return Mapper.Map<EmployeeServiceResponse>(entity);
        }

        protected override IQueryable<Database.EmployeeService> ApplyFilters(IQueryable<Database.EmployeeService> query, EmployeeServiceSearchObject? search)
        {
            if (search != null)
            {
                if (search.EmployeeId.HasValue)
                {
                    query = query.Where(es => es.EmployeeId == search.EmployeeId.Value);
                }

                if (search.WellnessServiceId.HasValue)
                {
                    query = query.Where(es => es.WellnessServiceId == search.WellnessServiceId.Value);
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(es => es.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.EmployeeService>> IncludeRelatedEntitiesAsync(IQueryable<Database.EmployeeService> query, EmployeeServiceSearchObject? search)
        {
            query = query
                .Include(es => es.Employee)
                    .ThenInclude(e => e.User)
                .Include(es => es.WellnessService);

            return Task.FromResult(query);
        }

        private async Task EnsureAssignmentIsUniqueAsync(int employeeId, int wellnessServiceId, int? excludedId = null)
        {
            var assignmentExists = await DbContext.EmployeeServices.AnyAsync(employeeService =>
                employeeService.EmployeeId == employeeId &&
                employeeService.WellnessServiceId == wellnessServiceId &&
                (!excludedId.HasValue || employeeService.Id != excludedId.Value));

            if (assignmentExists)
            {
                throw new BusinessException("This employee is already assigned to the selected service.");
            }
        }
    }
}
