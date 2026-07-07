using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;

namespace ZenCare.Services.Mapping
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Database.Supplier, SupplierResponse>();
            CreateMap<SupplierInsertRequest, Database.Supplier>();
            CreateMap<SupplierUpdateRequest, Database.Supplier>();
        }
    }
}
