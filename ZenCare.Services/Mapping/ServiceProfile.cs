using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Database.WellnessService, ServiceResponse>()
                .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.ServiceCategory.Name));
            CreateMap<ServiceInsertRequest, Database.WellnessService>();
            CreateMap<ServiceUpdateRequest, Database.WellnessService>();
        }
    }
}
