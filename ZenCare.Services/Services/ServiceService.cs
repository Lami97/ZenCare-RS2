using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class ServiceService : BaseCRUDService<ServiceResponse, Database.WellnessService, ServiceInsertRequest, ServiceUpdateRequest, ServiceSearchObject>, IServiceService
    {
        public ServiceService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<ServiceResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.WellnessServices
                .Include(s => s.ServiceCategory)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.WellnessService), id);
            }

            return Mapper.Map<ServiceResponse>(entity);
        }

        protected override IQueryable<Database.WellnessService> ApplyFilters(IQueryable<Database.WellnessService> query, ServiceSearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(s => s.Name.Contains(search.Name));
                }

                if (search.ServiceCategoryId.HasValue)
                {
                    query = query.Where(s => s.ServiceCategoryId == search.ServiceCategoryId.Value);
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(s => s.Status == (search.IsActive.Value ? ServiceStatus.Active : ServiceStatus.Inactive));
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.WellnessService>> IncludeRelatedEntitiesAsync(IQueryable<Database.WellnessService> query, ServiceSearchObject? search)
        {
            query = query.Include(s => s.ServiceCategory);

            return Task.FromResult(query);
        }
    }
}
