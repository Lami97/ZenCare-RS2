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

        public async Task<PagedResult<AppointmentResponse>> GetMyAsync(int userId, AppointmentSearchObject? search)
        {
            search ??= new AppointmentSearchObject();
            search.UserId = userId;

            return await GetAllAsync(search);
        }

        public async Task<AppointmentResponse> GetMyByIdAsync(int id, int userId)
        {
            var entity = await GetClientAppointmentEntityAsync(id, userId);

            return Mapper.Map<AppointmentResponse>(entity);
        }

        public async Task<AppointmentResponse> InsertMyAsync(int userId, AppointmentInsertRequest request)
        {
            request.UserId = userId;

            return await InsertAsync(request);
        }

        public async Task<AppointmentResponse> UpdateMyAsync(int id, int userId, AppointmentUpdateRequest request)
        {
            await EnsureClientAppointmentExistsAsync(id, userId);

            request.Id = id;
            request.UserId = userId;

            return await UpdateAsync(id, request);
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

        private async Task<Database.Appointment> GetClientAppointmentEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Appointments
                .Include(a => a.User)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .Include(a => a.WellnessService)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            return entity;
        }

        private async Task EnsureClientAppointmentExistsAsync(int id, int userId)
        {
            var exists = await DbContext.Appointments
                .AnyAsync(a => a.Id == id && a.UserId == userId);

            if (!exists)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }
        }
    }
}
