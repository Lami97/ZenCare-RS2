using ZenCare.Model.Messages;

namespace ZenCare.Services.Interfaces
{
    public interface INotificationEventPublisher
    {
        Task PublishAsync(NotificationEventMessage message, CancellationToken cancellationToken = default);
    }
}
