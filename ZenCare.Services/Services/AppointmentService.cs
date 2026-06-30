using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class AppointmentService : BaseCRUDService<AppointmentResponse, Database.Appointment, AppointmentInsertRequest, AppointmentUpdateRequest, AppointmentSearchObject>, IAppointmentService
    {
        public AppointmentService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<AppointmentResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Appointments
                .Include(a => a.User)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .Include(a => a.WellnessService)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            return Mapper.Map<AppointmentResponse>(entity);
        }

        protected override IQueryable<Database.Appointment> ApplyFilters(IQueryable<Database.Appointment> query, AppointmentSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(a => a.UserId == search.UserId.Value);
                }

                if (search.EmployeeId.HasValue)
                {
                    query = query.Where(a => a.EmployeeId == search.EmployeeId.Value);
                }

                if (search.WellnessServiceId.HasValue)
                {
                    query = query.Where(a => a.WellnessServiceId == search.WellnessServiceId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(a => a.Status == search.Status.Value);
                }

                if (search.AppointmentDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate.Date == search.AppointmentDate.Value.Date);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Appointment>> IncludeRelatedEntitiesAsync(IQueryable<Database.Appointment> query, AppointmentSearchObject? search)
        {
            query = query
                .Include(a => a.User)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .Include(a => a.WellnessService);

            return Task.FromResult(query);
        }
    }
}
