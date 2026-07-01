using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class RecommendationLogProfile : Profile
    {
        public RecommendationLogProfile()
        {
            CreateMap<Database.RecommendationLog, RecommendationLogResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.WellnessService != null ? src.WellnessService.Name : null));
            CreateMap<RecommendationLogInsertRequest, Database.RecommendationLog>();
            CreateMap<RecommendationLogUpdateRequest, Database.RecommendationLog>();
        }
    }
}
