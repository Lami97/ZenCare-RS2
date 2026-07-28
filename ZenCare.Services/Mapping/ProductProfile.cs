using AutoMapper;
using ZenCare.Model.Enums;
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
                .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.UnitOfMeasure.Name))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == ProductStatus.Active));
            CreateMap<ProductInsertRequest, Database.Product>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? ProductStatus.Active : ProductStatus.Inactive));
            CreateMap<ProductUpdateRequest, Database.Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? ProductStatus.Active : ProductStatus.Inactive));
        }
    }
}
