using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IAppointmentService : ICRUDService<AppointmentResponse, AppointmentInsertRequest, AppointmentUpdateRequest, AppointmentSearchObject>
    {
    }
}
