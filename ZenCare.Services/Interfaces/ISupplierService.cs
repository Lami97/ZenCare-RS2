using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface ISupplierService : ICRUDService<SupplierResponse, SupplierInsertRequest, SupplierUpdateRequest, SupplierSearchObject>
    {
    }
}
