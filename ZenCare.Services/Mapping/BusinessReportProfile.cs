using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class BusinessReportProfile : Profile
    {
        public BusinessReportProfile()
        {
            CreateMap<Database.BusinessReport, BusinessReportResponse>()
                .ForMember(dest => dest.GeneratedByUserName, opt => opt.MapFrom(src => src.GeneratedByUser.Username));
            CreateMap<BusinessReportInsertRequest, Database.BusinessReport>();
            CreateMap<BusinessReportUpdateRequest, Database.BusinessReport>();
        }
    }
}
