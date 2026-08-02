using AutoMapper;
using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Database.WellnessService, ServiceResponse>()
                .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.ServiceCategory.Name))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == ServiceStatus.Active));
            CreateMap<ServiceInsertRequest, Database.WellnessService>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? ServiceStatus.Active : ServiceStatus.Inactive));
            CreateMap<ServiceUpdateRequest, Database.WellnessService>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? ServiceStatus.Active : ServiceStatus.Inactive));
        }
    }
}
