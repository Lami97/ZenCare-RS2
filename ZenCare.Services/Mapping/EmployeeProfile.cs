using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Database.Employee, EmployeeResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => (src.User.FirstName + " " + src.User.LastName).Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));
            CreateMap<EmployeeInsertRequest, Database.Employee>();
            CreateMap<EmployeeUpdateRequest, Database.Employee>();
        }
    }
}
