using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ClientProfileProfile : Profile
    {
        public ClientProfileProfile()
        {
            CreateMap<Database.ClientProfile, ClientProfileResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<ClientProfileInsertRequest, Database.ClientProfile>();
            CreateMap<ClientProfileUpdateRequest, Database.ClientProfile>();
        }
    }
}
