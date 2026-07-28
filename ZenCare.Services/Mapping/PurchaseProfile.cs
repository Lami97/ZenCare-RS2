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
                .ForMember(dest => dest.PurchaseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.PurchaseItems, opt => opt.MapFrom(src => src.PurchaseItems));
            CreateMap<PurchaseInsertRequest, Database.Purchase>();
            CreateMap<PurchaseUpdateRequest, Database.Purchase>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
