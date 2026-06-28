using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class FAQCategoryProfile : Profile
    {
        public FAQCategoryProfile()
        {
            CreateMap<Database.FAQCategory, FAQCategoryResponse>();
            CreateMap<FAQCategoryInsertRequest, Database.FAQCategory>();
            CreateMap<FAQCategoryUpdateRequest, Database.FAQCategory>();
        }
    }
}
