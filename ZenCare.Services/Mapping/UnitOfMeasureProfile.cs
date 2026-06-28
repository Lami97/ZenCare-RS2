using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class UnitOfMeasureProfile : Profile
    {
        public UnitOfMeasureProfile()
        {
            CreateMap<Database.UnitOfMeasure, UnitOfMeasureResponse>();
            CreateMap<UnitOfMeasureInsertRequest, Database.UnitOfMeasure>();
            CreateMap<UnitOfMeasureUpdateRequest, Database.UnitOfMeasure>();
        }
    }
}
