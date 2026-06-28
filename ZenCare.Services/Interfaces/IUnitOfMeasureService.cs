using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IUnitOfMeasureService : ICRUDService<UnitOfMeasureResponse, UnitOfMeasureInsertRequest, UnitOfMeasureUpdateRequest, UnitOfMeasureSearchObject>
    {
    }
}
