using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces
{
    public interface IFAQService : ICRUDService<FAQResponse, FAQInsertRequest, FAQUpdateRequest, FAQSearchObject>
    {
    }
}
