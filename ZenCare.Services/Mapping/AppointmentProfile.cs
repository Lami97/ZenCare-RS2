using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Database.Appointment, AppointmentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.User.Username))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.WellnessService.Name));
            CreateMap<AppointmentInsertRequest, Database.Appointment>();
            CreateMap<AppointmentUpdateRequest, Database.Appointment>();
        }
    }
}
