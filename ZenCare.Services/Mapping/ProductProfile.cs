using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Database.Product, ProductResponse>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory.Name))
                .ForMember(dest => dest.ProductTypeName, opt => opt.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.UnitOfMeasure.Name));
            CreateMap<ProductInsertRequest, Database.Product>();
            CreateMap<ProductUpdateRequest, Database.Product>();
        }
    }
}
