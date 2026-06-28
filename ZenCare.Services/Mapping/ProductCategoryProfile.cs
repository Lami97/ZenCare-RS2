using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ProductCategoryProfile : Profile
    {
        public ProductCategoryProfile()
        {
            CreateMap<Database.ProductCategory, ProductCategoryResponse>();
            CreateMap<ProductCategoryInsertRequest, Database.ProductCategory>();
            CreateMap<ProductCategoryUpdateRequest, Database.ProductCategory>();
        }
    }
}
