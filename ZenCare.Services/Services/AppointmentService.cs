using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
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

        public override async Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request)
        {
            await ValidateAppointmentRequestAsync(
                request.UserId,
                request.EmployeeId,
                request.WellnessServiceId,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime);
            ValidateAppointmentStatus(request.Status, request.CancellationReason);

            return await base.InsertAsync(request);
        }

        public override async Task<AppointmentResponse> UpdateAsync(int id, AppointmentUpdateRequest request)
        {
            var entity = await DbContext.Appointments.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            await ValidateAppointmentRequestAsync(
                request.UserId,
                request.EmployeeId,
                request.WellnessServiceId,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime,
                id);
            ValidateStatusTransition(entity.Status, request.Status);
            ValidateAppointmentStatus(request.Status, request.CancellationReason);

            Mapper.Map(request, entity);
            SetUpdatedAt(entity);

            await DbContext.SaveChangesAsync();

            return await GetByIdAsync(id);
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

        private async Task ValidateAppointmentRequestAsync(
            int userId,
            int employeeId,
            int wellnessServiceId,
            DateTime appointmentDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? currentAppointmentId = null)
        {
            if (endTime <= startTime)
            {
                throw new BusinessException("End time must be after start time.");
            }

            var userExists = await DbContext.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                throw new BusinessException("User was not found.");
            }

            var employee = await DbContext.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                throw new BusinessException("Employee was not found.");
            }

            if (!employee.IsAvailable)
            {
                throw new BusinessException("The selected employee is not available.");
            }

            var wellnessServiceExists = await DbContext.WellnessServices.AnyAsync(s => s.Id == wellnessServiceId);

            if (!wellnessServiceExists)
            {
                throw new BusinessException("Wellness service was not found.");
            }

            var hasOverlap = await DbContext.Appointments
                .AnyAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    (!currentAppointmentId.HasValue || a.Id != currentAppointmentId.Value) &&
                    a.AppointmentDate.Date == appointmentDate.Date &&
                    startTime < a.EndTime &&
                    endTime > a.StartTime);

            if (hasOverlap)
            {
                throw new BusinessException("The selected employee already has an appointment during this time.");
            }
        }

        private static void ValidateStatusTransition(AppointmentStatus currentStatus, AppointmentStatus newStatus)
        {
            if (currentStatus == newStatus)
            {
                return;
            }

            if (!IsValidTransition(currentStatus, newStatus))
            {
                throw new BusinessException($"Appointment status cannot be changed from {currentStatus} to {newStatus}.");
            }
        }

        private static bool IsValidTransition(AppointmentStatus currentStatus, AppointmentStatus newStatus)
        {
            return currentStatus switch
            {
                AppointmentStatus.Pending => newStatus is AppointmentStatus.Confirmed or AppointmentStatus.Cancelled,
                AppointmentStatus.Confirmed => newStatus is AppointmentStatus.Paid or AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.NoShow,
                AppointmentStatus.Paid => newStatus is AppointmentStatus.Completed or AppointmentStatus.Cancelled,
                AppointmentStatus.Completed => false,
                AppointmentStatus.Cancelled => false,
                AppointmentStatus.NoShow => false,
                _ => false
            };
        }

        private static void ValidateAppointmentStatus(AppointmentStatus status, string? cancellationReason)
        {
            if (!Enum.IsDefined(typeof(AppointmentStatus), status))
            {
                throw new BusinessException("Appointment status is not valid.");
            }

            if (status == AppointmentStatus.Cancelled && string.IsNullOrWhiteSpace(cancellationReason))
            {
                throw new BusinessException("Cancellation reason is required when cancelling an appointment.");
            }
        }
    }
}
