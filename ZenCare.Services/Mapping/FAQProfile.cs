using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class FAQProfile : Profile
    {
        public FAQProfile()
        {
            CreateMap<Database.FAQ, FAQResponse>()
                .ForMember(dest => dest.FAQCategoryName, opt => opt.MapFrom(src => src.FAQCategory.Name));
            CreateMap<FAQInsertRequest, Database.FAQ>();
            CreateMap<FAQUpdateRequest, Database.FAQ>();
        }
    }
}
