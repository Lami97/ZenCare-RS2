using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class ServiceCategoryProfile : Profile
    {
        public ServiceCategoryProfile()
        {
            CreateMap<Database.ServiceCategory, ServiceCategoryResponse>();
            CreateMap<ServiceCategoryInsertRequest, Database.ServiceCategory>();
            CreateMap<ServiceCategoryUpdateRequest, Database.ServiceCategory>();
        }
    }
}
