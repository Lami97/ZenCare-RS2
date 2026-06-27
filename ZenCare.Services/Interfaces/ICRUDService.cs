using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Interfaces;

public interface ICRUDService<T, TInsert, TUpdate, TSearch> : IReadService<T, TSearch>
    where TSearch : BaseSearchObject
{
    Task<T> InsertAsync(TInsert request);

    Task<T> UpdateAsync(int id, TUpdate request);

    Task DeleteAsync(int id);
}
