using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services;

public class TimeSlotService
    : BaseCRUDService<TimeSlotResponse, Database.TimeSlot, TimeSlotInsertRequest, TimeSlotUpdateRequest, TimeSlotSearchObject>,
        ITimeSlotService
{
    private readonly TimeZoneInfo _businessTimeZone;

    public TimeSlotService(
        ZenCareDbContext dbContext,
        IMapper mapper,
        TimeZoneInfo businessTimeZone) : base(dbContext, mapper)
    {
        _businessTimeZone = businessTimeZone;
    }

    public override async Task<PagedResult<TimeSlotResponse>> GetAllAsync(TimeSlotSearchObject? search = null)
    {
        search ??= new TimeSlotSearchObject();
        if (string.IsNullOrWhiteSpace(search.SortBy))
        {
            search.SortBy = "SlotDate, StartTime, Id";
        }

        var result = await base.GetAllAsync(search);
        foreach (var item in result.Items)
        {
            SetAvailability(item);
        }

        return result;
    }

    public async Task<PagedResult<TimeSlotResponse>> GetAvailableAsync(TimeSlotSearchObject? search)
    {
        search ??= new TimeSlotSearchObject();
        search.IsActive = true;
        search.IsAvailable = true;
        return await GetAllAsync(search);
    }

    public override async Task<TimeSlotResponse> GetByIdAsync(int id)
    {
        var entity = await DbContext.TimeSlots
            .AsNoTracking()
            .Include(slot => slot.Employee)
                .ThenInclude(employee => employee.User)
            .Include(slot => slot.WellnessService)
            .Include(slot => slot.Appointments)
            .FirstOrDefaultAsync(slot => slot.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Database.TimeSlot), id);
        }

        var response = Mapper.Map<TimeSlotResponse>(entity);
        SetAvailability(response);
        return response;
    }

    public override async Task<TimeSlotResponse> InsertAsync(TimeSlotInsertRequest request)
    {
        await ValidateRequestAsync(request, null);
        var result = await base.InsertAsync(request);
        return await GetByIdAsync(result.Id);
    }

    public override async Task<TimeSlotResponse> UpdateAsync(int id, TimeSlotUpdateRequest request)
    {
        var entity = await DbContext.TimeSlots
            .Include(slot => slot.Appointments)
            .FirstOrDefaultAsync(slot => slot.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Database.TimeSlot), id);
        }

        var scheduleChanged = entity.EmployeeId != request.EmployeeId ||
            entity.WellnessServiceId != request.WellnessServiceId ||
            entity.SlotDate.Date != request.SlotDate.Date ||
            entity.StartTime != request.StartTime ||
            entity.EndTime != request.EndTime;

        if (scheduleChanged && entity.Appointments.Count != 0)
        {
            throw new BusinessException("A schedule entry referenced by a reservation cannot have its employee, service or time changed. Deactivate it instead.");
        }

        if (scheduleChanged || (request.IsActive && !entity.IsActive))
        {
            await ValidateRequestAsync(request, id);
        }

        request.Id = id;
        Mapper.Map(request, entity);
        SetUpdatedAt(entity);
        await DbContext.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await DbContext.TimeSlots
            .Include(slot => slot.Appointments)
            .FirstOrDefaultAsync(slot => slot.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Database.TimeSlot), id);
        }

        if (entity.Appointments.Count != 0)
        {
            throw new BusinessException("A schedule entry with reservation history cannot be deleted. Deactivate it instead.");
        }

        DbContext.TimeSlots.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    protected override IQueryable<Database.TimeSlot> ApplyFilters(
        IQueryable<Database.TimeSlot> query,
        TimeSlotSearchObject? search)
    {
        if (search == null)
        {
            return query;
        }

        if (search.EmployeeId.HasValue)
        {
            query = query.Where(slot => slot.EmployeeId == search.EmployeeId.Value);
        }

        if (search.WellnessServiceId.HasValue)
        {
            query = query.Where(slot => slot.WellnessServiceId == search.WellnessServiceId.Value);
        }

        if (search.SlotDate.HasValue)
        {
            query = query.Where(slot => slot.SlotDate.Date == search.SlotDate.Value.Date);
        }

        if (search.IsActive.HasValue)
        {
            query = query.Where(slot => slot.IsActive == search.IsActive.Value);
        }

        if (search.Status.HasValue)
        {
            var now = GetBusinessNow();
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            query = search.Status.Value switch
            {
                TimeSlotStatus.Available => query.Where(slot =>
                    slot.IsActive &&
                    (slot.SlotDate.Date > today ||
                        (slot.SlotDate.Date == today && slot.StartTime > currentTime)) &&
                    !slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled)),
                TimeSlotStatus.Booked => query.Where(slot =>
                    slot.IsActive &&
                    slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled)),
                TimeSlotStatus.Inactive => query.Where(slot => !slot.IsActive),
                TimeSlotStatus.Expired => query.Where(slot =>
                    slot.IsActive &&
                    (slot.SlotDate.Date < today ||
                        (slot.SlotDate.Date == today && slot.StartTime <= currentTime)) &&
                    !slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled)),
                _ => query
            };
        }

        if (search.IsAvailable.HasValue)
        {
            var now = GetBusinessNow();
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            if (search.IsAvailable.Value)
            {
                query = query.Where(slot =>
                    slot.IsActive &&
                    (slot.SlotDate.Date > today ||
                        (slot.SlotDate.Date == today && slot.StartTime > currentTime)) &&
                    !slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled));
            }
            else
            {
                query = query.Where(slot =>
                    !slot.IsActive ||
                    slot.SlotDate.Date < today ||
                    (slot.SlotDate.Date == today && slot.StartTime <= currentTime) ||
                    slot.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled));
            }
        }

        return query;
    }

    protected override Task<IQueryable<Database.TimeSlot>> IncludeRelatedEntitiesAsync(
        IQueryable<Database.TimeSlot> query,
        TimeSlotSearchObject? search)
    {
        IQueryable<Database.TimeSlot> includedQuery = query
            .AsNoTracking()
            .Include(slot => slot.Employee)
                .ThenInclude(employee => employee.User)
            .Include(slot => slot.WellnessService)
            .Include(slot => slot.Appointments);

        return Task.FromResult(includedQuery);
    }

    private async Task ValidateRequestAsync(TimeSlotInsertRequest request, int? currentId)
    {
        if (request.EndTime <= request.StartTime)
        {
            throw new BusinessException("End time must be after start time.");
        }

        var slotStart = DateTime.SpecifyKind(request.SlotDate.Date.Add(request.StartTime), DateTimeKind.Unspecified);
        if (slotStart <= GetBusinessNow())
        {
            throw new BusinessException("Schedule start time must be in the future.");
        }

        var employee = await DbContext.Employees
            .Where(item => item.Id == request.EmployeeId)
            .Select(item => new { item.IsAvailable, UserIsActive = item.User.IsActive })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            throw new BusinessException("Employee was not found.");
        }

        if (!employee.IsAvailable || !employee.UserIsActive)
        {
            throw new BusinessException("The selected employee is inactive or unavailable.");
        }

        var serviceStatus = await DbContext.WellnessServices
            .Where(service => service.Id == request.WellnessServiceId)
            .Select(service => (ServiceStatus?)service.Status)
            .FirstOrDefaultAsync();

        if (!serviceStatus.HasValue)
        {
            throw new BusinessException("Wellness service was not found.");
        }

        if (serviceStatus.Value != ServiceStatus.Active)
        {
            throw new BusinessException("The selected service is inactive.");
        }

        var hasAssignment = await DbContext.EmployeeServices.AnyAsync(assignment =>
            assignment.EmployeeId == request.EmployeeId &&
            assignment.WellnessServiceId == request.WellnessServiceId &&
            assignment.IsActive);

        if (!hasAssignment)
        {
            throw new BusinessException("The selected employee is not assigned to the selected service.");
        }

        var slotDate = request.SlotDate.Date;
        var overlapsSlot = await DbContext.TimeSlots.AnyAsync(slot =>
            slot.EmployeeId == request.EmployeeId &&
            slot.IsActive &&
            (!currentId.HasValue || slot.Id != currentId.Value) &&
            slot.SlotDate.Date == slotDate &&
            request.StartTime < slot.EndTime &&
            request.EndTime > slot.StartTime);

        if (overlapsSlot)
        {
            throw new BusinessException("The selected employee already has an active schedule entry during this time.");
        }

        var overlapsReservation = await DbContext.Appointments.AnyAsync(appointment =>
            appointment.EmployeeId == request.EmployeeId &&
            appointment.Status != AppointmentStatus.Cancelled &&
            appointment.AppointmentDate.Date == slotDate &&
            request.StartTime < appointment.EndTime &&
            request.EndTime > appointment.StartTime &&
            (!currentId.HasValue || appointment.TimeSlotId != currentId.Value));

        if (overlapsReservation)
        {
            throw new BusinessException("The selected employee already has a reservation during this time.");
        }
    }

    private void SetAvailability(TimeSlotResponse response)
    {
        var start = DateTime.SpecifyKind(response.SlotDate.Date.Add(response.StartTime), DateTimeKind.Unspecified);
        response.IsAvailable = response.IsActive && !response.IsBooked && start > GetBusinessNow();
        response.Status = !response.IsActive
            ? "Inactive"
            : response.IsBooked
                ? "Booked"
                : start <= GetBusinessNow()
                    ? "Expired"
                    : "Available";
    }

    private DateTime GetBusinessNow() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _businessTimeZone);
}
