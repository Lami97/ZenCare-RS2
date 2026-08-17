using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Database.Review, ReviewResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.WellnessService.Name : null));
            CreateMap<ReviewInsertRequest, Database.Review>();
            CreateMap<ReviewUpdateRequest, Database.Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
