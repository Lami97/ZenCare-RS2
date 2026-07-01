using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class PurchaseItemProfile : Profile
    {
        public PurchaseItemProfile()
        {
            CreateMap<Database.PurchaseItem, PurchaseItemResponse>()
                .ForMember(dest => dest.PurchaseNumber, opt => opt.MapFrom(src => src.Purchase.PurchaseNumber))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<PurchaseItemInsertRequest, Database.PurchaseItem>();
            CreateMap<PurchaseItemUpdateRequest, Database.PurchaseItem>();
        }
    }
}
