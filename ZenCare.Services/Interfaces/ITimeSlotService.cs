using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces;

public interface ITimeSlotService : ICRUDService<TimeSlotResponse, TimeSlotInsertRequest, TimeSlotUpdateRequest, TimeSlotSearchObject>
{
    Task<PagedResult<TimeSlotResponse>> GetAvailableAsync(TimeSlotSearchObject? search);
}
