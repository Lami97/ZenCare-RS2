using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces;

public interface IReadService<T, TSearch>
    where TSearch : BaseSearchObject
{
    Task<PagedResult<T>> GetAllAsync(TSearch? search = null);

    Task<T> GetByIdAsync(int id);
}
