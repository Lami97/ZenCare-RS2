using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ProductTypeProfile : Profile
    {
        public ProductTypeProfile()
        {
            CreateMap<Database.ProductType, ProductTypeResponse>();
            CreateMap<ProductTypeInsertRequest, Database.ProductType>();
            CreateMap<ProductTypeUpdateRequest, Database.ProductType>();
        }
    }
}
