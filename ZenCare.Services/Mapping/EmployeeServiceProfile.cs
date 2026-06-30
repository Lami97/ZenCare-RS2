using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class EmployeeServiceProfile : Profile
    {
        public EmployeeServiceProfile()
        {
            CreateMap<Database.EmployeeService, EmployeeServiceResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.User.Username))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.WellnessService.Name));
            CreateMap<EmployeeServiceInsertRequest, Database.EmployeeService>();
            CreateMap<EmployeeServiceUpdateRequest, Database.EmployeeService>();
        }
    }
}
