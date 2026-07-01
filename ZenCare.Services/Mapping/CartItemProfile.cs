using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<Database.CartItem, CartItemResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Cart.User.Username))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<CartItemInsertRequest, Database.CartItem>();
            CreateMap<CartItemUpdateRequest, Database.CartItem>();
        }
    }
}
