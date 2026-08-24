using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Messages;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class AppointmentService : BaseCRUDService<AppointmentResponse, Database.Appointment, AppointmentInsertRequest, AppointmentUpdateRequest, AppointmentSearchObject>, IAppointmentService
    {
        private readonly INotificationEventPublisher _notificationEventPublisher;
        private readonly TimeZoneInfo _businessTimeZone;

        public AppointmentService(
            ZenCareDbContext dbContext,
            IMapper mapper,
            INotificationEventPublisher notificationEventPublisher,
            TimeZoneInfo businessTimeZone) : base(dbContext, mapper)
        {
            _notificationEventPublisher = notificationEventPublisher;
            _businessTimeZone = businessTimeZone;
        }

        public override async Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request)
        {
            return await BookTimeSlotAsync(request.UserId, request.TimeSlotId, request.Notes);
        }

        public override async Task<AppointmentResponse> UpdateAsync(int id, AppointmentUpdateRequest request)
        {
            return await UpdateCoreAsync(id, request, null);
        }

        public override Task DeleteAsync(int id) =>
            throw new BusinessException("Appointments cannot be deleted. Use the cancellation or status workflow.");

        public async Task<AppointmentResponse> UpdateWithActorAsync(
            int id,
            int actorUserId,
            AppointmentUpdateRequest request)
        {
            return await UpdateCoreAsync(id, request, actorUserId);
        }

        private async Task<AppointmentResponse> UpdateCoreAsync(
            int id,
            AppointmentUpdateRequest request,
            int? actorUserId)
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

            if (entity.UserId != request.UserId || isRescheduling || isEmployeeServiceChanged)
            {
                throw new BusinessException("Reservation client, employee, service and time cannot be changed. Cancel it and book another schedule entry instead.");
            }

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
            var previousStatus = entity.Status;

            ValidateStatusTransition(previousStatus, request.Status);
            ValidateAppointmentStatus(request.Status, request.CancellationReason);
            ValidateAppointmentStatusTiming(
                request.Status,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime);

            Mapper.Map(request, entity);
            SetUpdatedAt(entity);

            if (previousStatus != entity.Status)
            {
                DbContext.AppointmentStatusHistories.Add(new Database.AppointmentStatusHistory
                {
                    AppointmentId = entity.Id,
                    OldStatus = previousStatus,
                    NewStatus = entity.Status,
                    ChangedByUserId = actorUserId,
                    ChangedAt = entity.UpdatedAt ?? DateTime.UtcNow,
                    Description = GetAppointmentStatusChangeDescription(entity.Status),
                    Reason = entity.Status == AppointmentStatus.Cancelled
                        ? entity.CancellationReason
                        : null
                });
            }

            await DbContext.SaveChangesAsync();

            var result = await GetByIdAsync(id);

            if (previousStatus != result.Status)
            {
                await PublishAppointmentStatusChangedAsync(result);
            }

            return result;
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

        public async Task<AppointmentResponse> InsertMyAsync(int userId, AppointmentBookRequest request)
        {
            return await BookTimeSlotAsync(userId, request.TimeSlotId, request.Notes);
        }

        private async Task<AppointmentResponse> BookTimeSlotAsync(int userId, int timeSlotId, string? notes)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var userIsActive = await DbContext.Users
                .Where(user => user.Id == userId)
                .Select(user => (bool?)user.IsActive)
                .FirstOrDefaultAsync();

            if (!userIsActive.HasValue)
            {
                throw new BusinessException("User was not found.");
            }

            if (!userIsActive.Value)
            {
                throw new BusinessException("The client account is inactive.");
            }

            var slot = await DbContext.TimeSlots
                .Include(item => item.Employee)
                    .ThenInclude(employee => employee.User)
                .Include(item => item.WellnessService)
                .Include(item => item.Appointments)
                .FirstOrDefaultAsync(item => item.Id == timeSlotId);

            if (slot == null)
            {
                throw new BusinessException("The selected schedule entry was not found.");
            }

            await ValidateBookableSlotAsync(slot);

            var appointment = new Database.Appointment
            {
                UserId = userId,
                EmployeeId = slot.EmployeeId,
                WellnessServiceId = slot.WellnessServiceId,
                TimeSlotId = slot.Id,
                AppointmentDate = slot.SlotDate.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = AppointmentStatus.Pending,
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            DbContext.Appointments.Add(appointment);

            try
            {
                await DbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                throw new BusinessException("The selected schedule entry is no longer available.");
            }

            var result = await GetByIdAsync(appointment.Id);
            await PublishAppointmentBookedAsync(result);
            return result;
        }

        private async Task ValidateBookableSlotAsync(Database.TimeSlot slot)
        {
            if (!slot.IsActive)
            {
                throw new BusinessException("The selected schedule entry is inactive.");
            }

            if (!slot.Employee.IsAvailable || !slot.Employee.User.IsActive)
            {
                throw new BusinessException("The selected employee is inactive or unavailable.");
            }

            if (slot.WellnessService.Status != ServiceStatus.Active)
            {
                throw new BusinessException("The selected service is inactive.");
            }

            var slotStart = GetAppointmentStartDateTime(slot.SlotDate, slot.StartTime);
            if (slotStart <= GetBusinessNow())
            {
                throw new BusinessException("The selected schedule entry is no longer in the future.");
            }

            var hasAssignment = await DbContext.EmployeeServices.AnyAsync(assignment =>
                assignment.EmployeeId == slot.EmployeeId &&
                assignment.WellnessServiceId == slot.WellnessServiceId &&
                assignment.IsActive);

            if (!hasAssignment)
            {
                throw new BusinessException("The selected employee is not assigned to the selected service.");
            }

            if (slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled))
            {
                throw new BusinessException("The selected schedule entry has already been booked.");
            }

            var hasEmployeeOverlap = await DbContext.Appointments.AnyAsync(appointment =>
                appointment.EmployeeId == slot.EmployeeId &&
                appointment.Status != AppointmentStatus.Cancelled &&
                appointment.AppointmentDate.Date == slot.SlotDate.Date &&
                slot.StartTime < appointment.EndTime &&
                slot.EndTime > appointment.StartTime);

            if (hasEmployeeOverlap)
            {
                throw new BusinessException("The selected employee already has a reservation during this time.");
            }
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

            var previousStatus = entity.Status;
            entity.Status = AppointmentStatus.Cancelled;
            entity.CancellationReason = request.CancellationReason.Trim();
            entity.UpdatedAt = DateTime.UtcNow;

            DbContext.AppointmentStatusHistories.Add(new Database.AppointmentStatusHistory
            {
                AppointmentId = entity.Id,
                OldStatus = previousStatus,
                NewStatus = AppointmentStatus.Cancelled,
                ChangedByUserId = userId,
                ChangedAt = entity.UpdatedAt.Value,
                Description = "Appointment cancelled by client.",
                Reason = entity.CancellationReason
            });

            await DbContext.SaveChangesAsync();

            var result = await GetByIdAsync(entity.Id);
            await PublishAppointmentStatusChangedAsync(result);

            return result;
        }

        public async Task<PagedResult<AppointmentStatusHistoryResponse>> GetStatusHistoryAsync(
            int id,
            PagedSearchObject? search)
        {
            if (!await DbContext.Appointments.AnyAsync(appointment => appointment.Id == id))
            {
                throw new NotFoundException(nameof(Database.Appointment), id);
            }

            var query = DbContext.AppointmentStatusHistories
                .AsNoTracking()
                .Where(history => history.AppointmentId == id);

            int? totalCount = null;
            if (search?.IncludeTotalCount == true)
            {
                totalCount = await query.CountAsync();
            }

            var (page, pageSize) = BaseSearchObject.NormalizePagination(search);
            var items = await query
                .OrderBy(history => history.ChangedAt)
                .ThenBy(history => history.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(history => new AppointmentStatusHistoryResponse
                {
                    Id = history.Id,
                    AppointmentId = history.AppointmentId,
                    OldStatus = history.OldStatus,
                    NewStatus = history.NewStatus,
                    ChangedByUserId = history.ChangedByUserId,
                    ChangedByUsername = history.ChangedByUser == null
                        ? null
                        : history.ChangedByUser.Username,
                    ChangedAt = history.ChangedAt,
                    Description = history.Description,
                    Reason = history.Reason
                })
                .ToListAsync();

            return new PagedResult<AppointmentStatusHistoryResponse>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        private static string GetAppointmentStatusChangeDescription(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Confirmed => "Appointment confirmed.",
                AppointmentStatus.Paid => "Appointment marked paid.",
                AppointmentStatus.Completed => "Appointment marked completed.",
                AppointmentStatus.Cancelled => "Appointment cancelled by employee or administrator.",
                AppointmentStatus.NoShow => "Appointment marked as no-show.",
                _ => $"Appointment status changed to {status}."
            };
        }

        private Task PublishAppointmentBookedAsync(AppointmentResponse appointment)
        {
            return _notificationEventPublisher.PublishAsync(new NotificationEventMessage
            {
                UserId = appointment.UserId,
                EventKey = $"appointment-booked:{appointment.Id}",
                Title = "Appointment booked",
                Message = $"Appointment #{appointment.Id} for {appointment.AppointmentDate:yyyy-MM-dd} at {appointment.StartTime:hh\\:mm} was booked.",
                OccurredAt = appointment.CreatedAt
            });
        }

        private Task PublishAppointmentStatusChangedAsync(AppointmentResponse appointment)
        {
            var cancelled = appointment.Status == AppointmentStatus.Cancelled;

            return _notificationEventPublisher.PublishAsync(new NotificationEventMessage
            {
                UserId = appointment.UserId,
                EventKey = $"appointment-status:{appointment.Id}:{(int)appointment.Status}",
                Title = cancelled ? "Appointment cancelled" : "Appointment status updated",
                Message = cancelled
                    ? $"Appointment #{appointment.Id} was cancelled."
                    : $"Appointment #{appointment.Id} status changed to {appointment.Status}.",
                OccurredAt = appointment.UpdatedAt ?? DateTime.UtcNow
            });
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

        private void ValidateFutureAppointmentTime(DateTime appointmentDate, TimeSpan startTime)
        {
            var now = GetBusinessNow();
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

        private void ValidateAppointmentCancellation(Database.Appointment appointment, string? cancellationReason)
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

            if (GetAppointmentStartDateTime(appointment.AppointmentDate, appointment.StartTime) <= GetBusinessNow())
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

        private void ValidateAppointmentStatusTiming(
            AppointmentStatus status,
            DateTime appointmentDate,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var now = GetBusinessNow();

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

        private DateTime GetBusinessNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _businessTimeZone);
        }
    }
}
