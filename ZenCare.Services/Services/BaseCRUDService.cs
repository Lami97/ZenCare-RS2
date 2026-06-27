using AutoMapper;
using ZenCare.Model.Exceptions;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services;

public abstract class BaseCRUDService<TModel, TDb, TInsert, TUpdate, TSearch>
    : BaseReadService<TModel, TDb, TSearch>, ICRUDService<TModel, TInsert, TUpdate, TSearch>
    where TDb : class
    where TSearch : BaseSearchObject
{
    protected BaseCRUDService(ZenCareDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public virtual async Task<TModel> InsertAsync(TInsert request)
    {
        var entity = Mapper.Map<TDb>(request);

        SetCreatedAt(entity);

        DbContext.Set<TDb>().Add(entity);
        await DbContext.SaveChangesAsync();

        return Mapper.Map<TModel>(entity);
    }

    public virtual async Task<TModel> UpdateAsync(int id, TUpdate request)
    {
        var entity = await DbContext.Set<TDb>().FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(typeof(TDb).Name, id);
        }

        Mapper.Map(request, entity);
        SetUpdatedAt(entity);

        await DbContext.SaveChangesAsync();

        return Mapper.Map<TModel>(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await DbContext.Set<TDb>().FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(typeof(TDb).Name, id);
        }

        DbContext.Set<TDb>().Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    protected virtual void SetCreatedAt(TDb entity)
    {
        var createdAtProperty = entity.GetType().GetProperty("CreatedAt");

        if (createdAtProperty?.CanWrite == true)
        {
            createdAtProperty.SetValue(entity, DateTime.UtcNow);
        }
    }

    protected virtual void SetUpdatedAt(TDb entity)
    {
        var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");

        if (updatedAtProperty?.CanWrite == true)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }
    }
}
