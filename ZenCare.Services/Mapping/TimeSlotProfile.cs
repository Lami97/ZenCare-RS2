using AutoMapper;
using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping;

public class TimeSlotProfile : Profile
{
    public TimeSlotProfile()
    {
        CreateMap<Database.TimeSlot, TimeSlotResponse>()
            .ForMember(destination => destination.EmployeeName, options => options.MapFrom(source =>
                (source.Employee.User.FirstName + " " + source.Employee.User.LastName).Trim()))
            .ForMember(destination => destination.ServiceName, options => options.MapFrom(source => source.WellnessService.Name))
            .ForMember(destination => destination.IsBooked, options => options.MapFrom(source =>
                source.Appointments.Any(appointment => appointment.Status != AppointmentStatus.Cancelled)))
            .ForMember(destination => destination.IsAvailable, options => options.Ignore())
            .ForMember(destination => destination.Status, options => options.Ignore());

        CreateMap<TimeSlotInsertRequest, Database.TimeSlot>();
        CreateMap<TimeSlotUpdateRequest, Database.TimeSlot>();
    }
}
