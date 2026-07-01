using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class BusinessReportService : BaseCRUDService<BusinessReportResponse, Database.BusinessReport, BusinessReportInsertRequest, BusinessReportUpdateRequest, BusinessReportSearchObject>, IBusinessReportService
    {
        public BusinessReportService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<BusinessReportResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.BusinessReports
                .Include(br => br.GeneratedByUser)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.BusinessReport), id);
            }

            return Mapper.Map<BusinessReportResponse>(entity);
        }

        protected override IQueryable<Database.BusinessReport> ApplyFilters(IQueryable<Database.BusinessReport> query, BusinessReportSearchObject? search)
        {
            if (search != null)
            {
                if (search.GeneratedByUserId.HasValue)
                {
                    query = query.Where(br => br.GeneratedByUserId == search.GeneratedByUserId.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.ReportType))
                {
                    query = query.Where(br => br.ReportType.Contains(search.ReportType));
                }

                if (search.DateFrom.HasValue)
                {
                    query = query.Where(br => br.DateFrom.Date >= search.DateFrom.Value.Date);
                }

                if (search.DateTo.HasValue)
                {
                    query = query.Where(br => br.DateTo.Date <= search.DateTo.Value.Date);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.BusinessReport>> IncludeRelatedEntitiesAsync(IQueryable<Database.BusinessReport> query, BusinessReportSearchObject? search)
        {
            query = query.Include(br => br.GeneratedByUser);

            return Task.FromResult(query);
        }
    }
}
