using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Database.Cart, CartResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<CartInsertRequest, Database.Cart>();
            CreateMap<CartUpdateRequest, Database.Cart>();
        }
    }
}
