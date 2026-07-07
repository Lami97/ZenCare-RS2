using AutoMapper;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class SupplierService : BaseCRUDService<SupplierResponse, Database.Supplier, SupplierInsertRequest, SupplierUpdateRequest, SupplierSearchObject>, ISupplierService
    {
        public SupplierService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        protected override IQueryable<Database.Supplier> ApplyFilters(IQueryable<Database.Supplier> query, SupplierSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(s => s.Name.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(s => s.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
