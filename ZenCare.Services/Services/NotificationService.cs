using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Exceptions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class NotificationService : BaseCRUDService<NotificationResponse, Database.Notification, NotificationInsertRequest, NotificationUpdateRequest, NotificationSearchObject>, INotificationService
    {
        public NotificationService(ZenCareDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<NotificationResponse> GetByIdAsync(int id)
        {
            var entity = await DbContext.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Database.Notification), id);
            }

            return Mapper.Map<NotificationResponse>(entity);
        }

        protected override IQueryable<Database.Notification> ApplyFilters(IQueryable<Database.Notification> query, NotificationSearchObject? search)
        {
            if (search != null)
            {
                if (search.UserId.HasValue)
                {
                    query = query.Where(n => n.UserId == search.UserId.Value);
                }

                if (search.Status.HasValue)
                {
                    query = query.Where(n => n.Status == search.Status.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.NotificationType))
                {
                    query = query.Where(n => n.NotificationType != null && n.NotificationType.Contains(search.NotificationType));
                }
            }

            return query;
        }

        protected override Task<IQueryable<Database.Notification>> IncludeRelatedEntitiesAsync(IQueryable<Database.Notification> query, NotificationSearchObject? search)
        {
            query = query.Include(n => n.User);

            return Task.FromResult(query);
        }
    }
}
