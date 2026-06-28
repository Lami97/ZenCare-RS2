using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IProductTypeService : ICRUDService<ProductTypeResponse, ProductTypeInsertRequest, ProductTypeUpdateRequest, ProductTypeSearchObject>
    {
    }
}
