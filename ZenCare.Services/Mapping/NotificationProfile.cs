using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Database.Notification, NotificationResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<NotificationInsertRequest, Database.Notification>();
            CreateMap<NotificationUpdateRequest, Database.Notification>();
        }
    }
}
