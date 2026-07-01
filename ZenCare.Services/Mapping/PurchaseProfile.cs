using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<Database.Purchase, PurchaseResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<PurchaseInsertRequest, Database.Purchase>();
            CreateMap<PurchaseUpdateRequest, Database.Purchase>();
        }
    }
}
