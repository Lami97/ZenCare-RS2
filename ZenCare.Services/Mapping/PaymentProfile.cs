using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Database.Payment, PaymentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<PaymentInsertRequest, Database.Payment>();
            CreateMap<PaymentUpdateRequest, Database.Payment>();
        }
    }
}
