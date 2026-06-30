using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class PaymentService : BaseCRUDService<PaymentResponse, Database.Payment, PaymentInsertRequest, PaymentUpdateRequest, PaymentSearchObject>, IPaymentService
    {
        public PaymentService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<PaymentResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Payment), id);
            }

            return Mapper.Map<PaymentResponse>(entity);
        }

        protected override IQueryable<Database.Payment> ApplyFilters(IQueryable<Database.Payment> query, PaymentSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(p => p.UserId == search.UserId.Value);
                }

                if (search.AppointmentId.HasValue)
                {
                    query = query.Where(p => p.AppointmentId == search.AppointmentId.Value);
                }

                if (search.PurchaseId.HasValue)
                {
                    query = query.Where(p => p.PurchaseId == search.PurchaseId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(p => p.Status == search.Status.Value);
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Payment>> IncludeRelatedEntitiesAsync(IQueryable<Database.Payment> query, PaymentSearchObject? search)
        {
            query = query.Include(p => p.User);

            return Task.FromResult(query);
        }
    }
}
