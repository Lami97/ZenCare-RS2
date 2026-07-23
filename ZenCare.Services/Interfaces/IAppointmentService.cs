using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IAppointmentService : ICRUDService<AppointmentResponse, AppointmentInsertRequest, AppointmentUpdateRequest, AppointmentSearchObject>
    {
        Task<PagedResult<AppointmentResponse>> GetMyAsync(int userId, AppointmentSearchObject? search);
        Task<AppointmentResponse> GetMyByIdAsync(int id, int userId);
        Task<AppointmentResponse> InsertMyAsync(int userId, AppointmentInsertRequest request);
        Task<AppointmentResponse> UpdateMyAsync(int id, int userId, AppointmentUpdateRequest request);
    }
}
