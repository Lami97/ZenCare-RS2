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
                .ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace((src.User.FirstName + " " + src.User.LastName).Trim())
                        ? src.User.Username
                        : (src.User.FirstName + " " + src.User.LastName).Trim()))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace((src.Employee.User.FirstName + " " + src.Employee.User.LastName).Trim())
                        ? src.Employee.User.Username
                        : (src.Employee.User.FirstName + " " + src.Employee.User.LastName).Trim()))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.WellnessService.Name))
                .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.WellnessService.ServiceCategory.Name));
            CreateMap<AppointmentInsertRequest, Database.Appointment>();
            CreateMap<AppointmentUpdateRequest, Database.Appointment>();
        }
    }
}
