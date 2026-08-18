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
                request.EndTime,
                enforceFutureSchedule: true);
            ValidateAppointmentStatus(request.Status, request.CancellationReason);
            ValidateAppointmentStatusTiming(
                request.Status,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime);

            return await base.InsertAsync(request);
        }

        public override async Task<AppointmentResponse> UpdateAsync(int id, AppointmentUpdateRequest request)
        {
            var entity = await DbContext.Appointments.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            var isRescheduling = IsScheduleChanged(entity, request.AppointmentDate, request.StartTime, request.EndTime);
            var isEmployeeChanged = entity.EmployeeId != request.EmployeeId;
            var isEmployeeServiceChanged = isEmployeeChanged ||
                entity.WellnessServiceId != request.WellnessServiceId;

            await ValidateAppointmentRequestAsync(
                request.UserId,
                request.EmployeeId,
                request.WellnessServiceId,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime,
                id,
                isRescheduling,
                isEmployeeServiceChanged,
                isEmployeeChanged,
                isEmployeeServiceChanged);
            ValidateStatusTransition(entity.Status, request.Status);
            ValidateAppointmentStatus(request.Status, request.CancellationReason);
            ValidateAppointmentStatusTiming(
                request.Status,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime);

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
            if (request.Status != AppointmentStatus.Pending)
            {
                throw new BusinessException("Client appointments must be created in Pending status.");
            }

            request.UserId = userId;

            return await InsertAsync(request);
        }

        public async Task<AppointmentResponse> UpdateMyAsync(int id, int userId, AppointmentUpdateRequest request)
        {
            var currentStatus = await DbContext.Appointments
                .Where(a => a.Id == id && a.UserId == userId)
                .Select(a => (AppointmentStatus?)a.Status)
                .FirstOrDefaultAsync();

            if (!currentStatus.HasValue)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            if (request.Status != currentStatus.Value)
            {
                throw new BusinessException("Appointment status cannot be changed through the client update endpoint. Use the cancellation action to cancel an appointment.");
            }

            request.Id = id;
            request.UserId = userId;

            return await UpdateAsync(id, request);
        }

        public async Task<AppointmentResponse> CancelMyAsync(int id, int userId, AppointmentCancelRequest request)
        {
            var entity = await DbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            ValidateAppointmentCancellation(entity, request.CancellationReason);

            entity.Status = AppointmentStatus.Cancelled;
            entity.CancellationReason = request.CancellationReason.Trim();
            entity.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<List<AppointmentEmployeeOptionResponse>> GetAvailableEmployeeOptionsAsync(
            int wellnessServiceId,
            DateTime? appointmentDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            int? page,
            int? pageSize)
        {
            var serviceExists = await DbContext.WellnessServices
                .AnyAsync(s => s.Id == wellnessServiceId);

            if (!serviceExists)
            {
                throw new BusinessException("Wellness service was not found.");
            }

            var serviceIsActive = await DbContext.WellnessServices
                .AnyAsync(s => s.Id == wellnessServiceId && s.Status == ServiceStatus.Active);

            if (!serviceIsActive)
            {
                return new List<AppointmentEmployeeOptionResponse>();
            }

            if ((appointmentDate.HasValue || startTime.HasValue || endTime.HasValue) &&
                !(appointmentDate.HasValue && startTime.HasValue && endTime.HasValue))
            {
                throw new BusinessException("Appointment date, start time and end time are required for availability filtering.");
            }

            if (startTime.HasValue && endTime.HasValue && endTime.Value <= startTime.Value)
            {
                throw new BusinessException("End time must be after start time.");
            }

            var query = DbContext.EmployeeServices
                .AsNoTracking()
                .Where(es =>
                    es.WellnessServiceId == wellnessServiceId &&
                    es.IsActive &&
                    es.Employee.IsAvailable &&
                    es.Employee.User.IsActive)
                .Select(es => es.Employee)
                .Distinct();

            if (appointmentDate.HasValue && startTime.HasValue && endTime.HasValue)
            {
                var date = appointmentDate.Value.Date;
                var requestedStart = startTime.Value;
                var requestedEnd = endTime.Value;

                query = query.Where(employee => !DbContext.Appointments.Any(a =>
                    a.EmployeeId == employee.Id &&
                    a.Status != AppointmentStatus.Cancelled &&
                    a.AppointmentDate.Date == date &&
                    requestedStart < a.EndTime &&
                    requestedEnd > a.StartTime));
            }

            var pagination = new BaseSearchObject
            {
                Page = page,
                PageSize = pageSize
            };

            var (normalizedPage, normalizedPageSize) = BaseSearchObject.NormalizePagination(pagination);

            var employees = await query
                .OrderBy(employee => employee.User.FirstName)
                .ThenBy(employee => employee.User.LastName)
                .ThenBy(employee => employee.Id)
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .Select(employee => new
                {
                    employee.Id,
                    employee.Specialization,
                    employee.IsAvailable,
                    employee.User.FirstName,
                    employee.User.LastName,
                    employee.User.Username
                })
                .ToListAsync();

            return employees
                .Select(employee => new AppointmentEmployeeOptionResponse
                {
                    EmployeeId = employee.Id,
                    FullName = BuildEmployeeDisplayName(employee.FirstName, employee.LastName, employee.Username),
                    Specialization = employee.Specialization,
                    IsAvailable = employee.IsAvailable
                })
                .ToList();
        }
        public override async Task<AppointmentResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Appointments
                .Include(a => a.User)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .Include(a => a.WellnessService)
                    .ThenInclude(s => s.ServiceCategory)
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
                .Include(a => a.WellnessService)
                    .ThenInclude(s => s.ServiceCategory);

            return Task.FromResult(query);
        }

        private static string BuildEmployeeDisplayName(string firstName, string lastName, string username)
        {
            var fullName = $"{firstName} {lastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? username : fullName;
        }
        private async Task<Database.Appointment> GetClientAppointmentEntityAsync(int id, int userId)
        {
            var entity = await DbContext.Appointments
                .Include(a => a.User)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .Include(a => a.WellnessService)
                    .ThenInclude(s => s.ServiceCategory)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            return entity;
        }

        private async Task ValidateAppointmentRequestAsync(
            int userId,
            int employeeId,
            int wellnessServiceId,
            DateTime appointmentDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? currentAppointmentId = null,
            bool enforceFutureSchedule = false,
            bool validateEmployeeServiceAssignment = true,
            bool validateEmployeeAccountStatus = true,
            bool validateServiceStatus = true)
        {
            if (endTime <= startTime)
            {
                throw new BusinessException("End time must be after start time.");
            }

            if (enforceFutureSchedule)
            {
                ValidateFutureAppointmentTime(appointmentDate, startTime);
            }

            var userExists = await DbContext.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                throw new BusinessException("User was not found.");
            }

            var employee = await DbContext.Employees
                .Where(e => e.Id == employeeId)
                .Select(e => new
                {
                    e.IsAvailable,
                    IsUserActive = e.User.IsActive
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                throw new BusinessException("Employee was not found.");
            }

            if (!employee.IsAvailable)
            {
                throw new BusinessException("The selected employee is not available.");
            }

            if (validateEmployeeAccountStatus && !employee.IsUserActive)
            {
                throw new BusinessException("The selected employee account is inactive.");
            }

            var wellnessService = await DbContext.WellnessServices
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == wellnessServiceId);

            if (wellnessService == null)
            {
                throw new BusinessException("Wellness service was not found.");
            }

            if (validateServiceStatus && wellnessService.Status != ServiceStatus.Active)
            {
                throw new BusinessException("The selected service is inactive.");
            }

            if (validateEmployeeServiceAssignment)
            {
                var hasActiveAssignment = await DbContext.EmployeeServices.AnyAsync(employeeService =>
                    employeeService.EmployeeId == employeeId &&
                    employeeService.WellnessServiceId == wellnessServiceId &&
                    employeeService.IsActive);

                if (!hasActiveAssignment)
                {
                    throw new BusinessException("The selected employee is not assigned to the selected service.");
                }
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

        private static void ValidateFutureAppointmentTime(DateTime appointmentDate, TimeSpan startTime)
        {
            var now = DateTime.Now;
            var appointmentDay = appointmentDate.Date;

            if (appointmentDay < now.Date)
            {
                throw new BusinessException("Appointment date cannot be in the past.");
            }

            var appointmentStart = DateTime.SpecifyKind(appointmentDay.Add(startTime), DateTimeKind.Unspecified);

            if (appointmentStart <= now)
            {
                throw new BusinessException("Start time must be in the future.");
            }
        }

        private static void ValidateAppointmentCancellation(Database.Appointment appointment, string? cancellationReason)
        {
            if (string.IsNullOrWhiteSpace(cancellationReason))
            {
                throw new BusinessException("Cancellation reason is required.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessException("Appointment was already cancelled.");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                throw new BusinessException("Completed appointments cannot be cancelled.");
            }

            if (appointment.Status is not (AppointmentStatus.Pending or AppointmentStatus.Confirmed))
            {
                throw new BusinessException("Only future pending or confirmed appointments can be cancelled.");
            }

            if (GetAppointmentStartDateTime(appointment.AppointmentDate, appointment.StartTime) <= DateTime.Now)
            {
                throw new BusinessException("Only future pending or confirmed appointments can be cancelled.");
            }
        }

        private static DateTime GetAppointmentStartDateTime(DateTime appointmentDate, TimeSpan startTime)
        {
            return DateTime.SpecifyKind(appointmentDate.Date.Add(startTime), DateTimeKind.Unspecified);
        }

        private static DateTime GetAppointmentEndDateTime(DateTime appointmentDate, TimeSpan endTime)
        {
            return DateTime.SpecifyKind(appointmentDate.Date.Add(endTime), DateTimeKind.Unspecified);
        }

        private static bool IsScheduleChanged(Database.Appointment appointment, DateTime appointmentDate, TimeSpan startTime, TimeSpan endTime)
        {
            return appointment.AppointmentDate.Date != appointmentDate.Date
                || appointment.StartTime != startTime
                || appointment.EndTime != endTime;
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

        private static void ValidateAppointmentStatusTiming(
            AppointmentStatus status,
            DateTime appointmentDate,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var now = DateTime.Now;

            if (status == AppointmentStatus.Completed &&
                GetAppointmentEndDateTime(appointmentDate, endTime) > now)
            {
                throw new BusinessException("An appointment cannot be completed before it has ended.");
            }

            if (status == AppointmentStatus.NoShow &&
                GetAppointmentStartDateTime(appointmentDate, startTime) > now)
            {
                throw new BusinessException("An appointment cannot be marked as no-show before its scheduled time.");
            }
        }
    }
}
