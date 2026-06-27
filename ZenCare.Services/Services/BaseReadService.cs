using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ZenCare.Model.Exceptions;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services;

public abstract class BaseReadService<TModel, TDb, TSearch> : IReadService<TModel, TSearch>
    where TDb : class
    where TSearch : BaseSearchObject
{
    protected readonly ZenCareDbContext DbContext;
    protected readonly IMapper Mapper;

    protected BaseReadService(ZenCareDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
    }

    public virtual async Task<ZenCare.Model.Responses.PagedResult<TModel>> GetAllAsync(TSearch? search = null)
    {
        IQueryable<TDb> query = DbContext.Set<TDb>();

        query = await IncludeRelatedEntitiesAsync(query, search);
        query = ApplyFilters(query, search);

        int? totalCount = null;

        if (search?.IncludeTotalCount == true)
        {
            totalCount = await query.CountAsync();
        }

        if (!string.IsNullOrWhiteSpace(search?.SortBy))
        {
            query = query.OrderBy(search.SortBy);
        }

        if (search is PagedSearchObject pagedSearch)
        {
            if (pagedSearch.Page.HasValue && pagedSearch.PageSize.HasValue)
            {
                query = query.Skip((pagedSearch.Page.Value - 1) * pagedSearch.PageSize.Value);
            }

            if (pagedSearch.PageSize.HasValue)
            {
                query = query.Take(pagedSearch.PageSize.Value);
            }
        }

        var entities = await query.ToListAsync();

        return new ZenCare.Model.Responses.PagedResult<TModel>
        {
            Items = Mapper.Map<List<TModel>>(entities),
            TotalCount = totalCount
        };
    }

    public virtual async Task<TModel> GetByIdAsync(int id)
    {
        var entity = await DbContext.Set<TDb>().FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(typeof(TDb).Name, id);
        }

        return Mapper.Map<TModel>(entity);
    }

    protected virtual IQueryable<TDb> ApplyFilters(IQueryable<TDb> query, TSearch? search)
    {
        return query;
    }

    protected virtual Task<IQueryable<TDb>> IncludeRelatedEntitiesAsync(IQueryable<TDb> query, TSearch? search)
    {
        return Task.FromResult(query);
    }
}
