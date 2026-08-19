using System.Text.Json;
using Microsoft.Extensions.Logging;
using ZenCare.Model.Messages;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class NotificationEventPublisher : INotificationEventPublisher
    {
        private const string RoutingKey = "notification";
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<NotificationEventPublisher> _logger;

        public NotificationEventPublisher(
            IRabbitMqService rabbitMqService,
            ILogger<NotificationEventPublisher> logger)
        {
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }

        public async Task PublishAsync(NotificationEventMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = JsonSerializer.Serialize(message);
                await _rabbitMqService.PublishAsync(RoutingKey, payload, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business operation completed, but notification event {EventKey} could not be published.",
                    message.EventKey);
            }
        }
    }
}
