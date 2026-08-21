using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;

namespace ZenCare.Services.Services;

public abstract class CachedReferenceCRUDService<TModel, TDb, TInsert, TUpdate, TSearch>
    : BaseCRUDService<TModel, TDb, TInsert, TUpdate, TSearch>
    where TDb : class
    where TSearch : BaseSearchObject
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly IMemoryCache _cache;
    private readonly string _cachePrefix = $"reference:{typeof(TDb).FullName}";

    protected CachedReferenceCRUDService(
        ZenCareDbContext dbContext,
        IMapper mapper,
        IMemoryCache cache)
        : base(dbContext, mapper)
    {
        _cache = cache;
    }

    public override async Task<PagedResult<TModel>> GetAllAsync(TSearch? search = null)
    {
        var cacheKey = $"{_cachePrefix}:{GetGeneration()}:list:{JsonSerializer.Serialize(search)}";
        var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await base.GetAllAsync(search);
        });

        return result!;
    }

    public override async Task<TModel> GetByIdAsync(int id)
    {
        var cacheKey = $"{_cachePrefix}:{GetGeneration()}:id:{id}";
        var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await base.GetByIdAsync(id);
        });

        return result!;
    }

    public override async Task<TModel> InsertAsync(TInsert request)
    {
        var result = await base.InsertAsync(request);
        InvalidateCache();
        return result;
    }

    public override async Task<TModel> UpdateAsync(int id, TUpdate request)
    {
        var result = await base.UpdateAsync(id, request);
        InvalidateCache();
        return result;
    }

    public override async Task DeleteAsync(int id)
    {
        await base.DeleteAsync(id);
        InvalidateCache();
    }

    private string GetGeneration()
    {
        var generationKey = $"{_cachePrefix}:generation";
        return _cache.GetOrCreate(generationKey, entry =>
        {
            entry.Priority = CacheItemPriority.NeverRemove;
            return Guid.NewGuid().ToString("N");
        })!;
    }

    private void InvalidateCache()
    {
        _cache.Remove($"{_cachePrefix}:generation");
    }
}
