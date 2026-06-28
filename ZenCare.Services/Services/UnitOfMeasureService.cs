using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class UnitOfMeasureService : BaseCRUDService<UnitOfMeasureResponse, Database.UnitOfMeasure, UnitOfMeasureInsertRequest, UnitOfMeasureUpdateRequest, UnitOfMeasureSearchObject>, IUnitOfMeasureService
    {
        public UnitOfMeasureService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.UnitOfMeasure> ApplyFilters(IQueryable<Database.UnitOfMeasure> query, UnitOfMeasureSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(uom => uom.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(uom => uom.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
