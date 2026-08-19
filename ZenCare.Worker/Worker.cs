using ZenCare.Services.Interfaces;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Model.Messages;
using ZenCare.Services;
using ZenCare.Services.Database;
using ZenCare.Services.Services;

namespace ZenCare.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(ILogger<Worker> logger, IRabbitMqService rabbitMqService, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retryDelay = TimeSpan.FromSeconds(1);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _rabbitMqService.InitializeAsync(stoppingToken);

                if (!_rabbitMqService.IsConnected)
                {
                    throw new InvalidOperationException("RabbitMQ connection is unavailable.");
                }

                await _rabbitMqService.RegisterConsumerAsync(
                    RabbitMqService.PurchaseQueueName,
                    ProcessPurchaseCreatedMessageAsync,
                    stoppingToken);
                await _rabbitMqService.RegisterConsumerAsync(
                    RabbitMqService.NotificationQueueName,
                    ProcessNotificationEventMessageAsync,
                    stoppingToken);

                _logger.LogInformation(
                    "ZenCare Worker is consuming queues {PurchaseQueueName} and {NotificationQueueName}.",
                    RabbitMqService.PurchaseQueueName,
                    RabbitMqService.NotificationQueueName);

                retryDelay = TimeSpan.FromSeconds(1);

                while (_rabbitMqService.IsConnected && !stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }

                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("RabbitMQ connection was lost. Worker will reconnect.");
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "RabbitMQ consumers are unavailable. Retrying in {DelaySeconds} seconds.",
                    retryDelay.TotalSeconds);
            }

            try
            {
                await Task.Delay(retryDelay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 8));
        }

        _logger.LogInformation("ZenCare Worker is stopping.");
    }

    private async Task ProcessPurchaseCreatedMessageAsync(string message, CancellationToken cancellationToken)
    {
        var purchaseCreated = JsonSerializer.Deserialize<PurchaseCreatedMessage>(message);

        if (purchaseCreated == null)
        {
            throw new InvalidDataException("Purchase message could not be deserialized.");
        }

        ValidatePurchaseCreatedMessage(purchaseCreated);

        _logger.LogInformation(
            "Processing purchase event. PurchaseId: {PurchaseId}, PurchaseNumber: {PurchaseNumber}, UserId: {UserId}, TotalAmount: {TotalAmount}",
            purchaseCreated.PurchaseId,
            purchaseCreated.PurchaseNumber,
            purchaseCreated.UserId,
            purchaseCreated.TotalAmount);

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ZenCareDbContext>();

        if (dbContext == null)
        {
            throw new InvalidOperationException("Worker database configuration is missing.");
        }

        var notificationMessage = $"Purchase {purchaseCreated.PurchaseNumber} was created successfully and is awaiting payment.";
        var notificationExists = await dbContext.Notifications.AnyAsync(
            n => n.UserId == purchaseCreated.UserId
                && n.NotificationType == "Purchase"
                && n.Message == notificationMessage,
            cancellationToken);

        if (notificationExists)
        {
            _logger.LogInformation("Notification for purchase event {PurchaseId} already exists. Skipping duplicate.", purchaseCreated.PurchaseId);
            return;
        }

        var userExists = await dbContext.Users.AnyAsync(u => u.Id == purchaseCreated.UserId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidDataException("Purchase message references a user that does not exist.");
        }

        dbContext.Notifications.Add(new Notification
        {
            UserId = purchaseCreated.UserId,
            Title = "Purchase created",
            Message = notificationMessage,
            NotificationType = "Purchase",
            Status = NotificationStatus.Sent,
            SentAt = DateTime.UtcNow,
            CreatedAt = purchaseCreated.CreatedAt
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Notification created for purchase event {PurchaseId}.", purchaseCreated.PurchaseId);
    }

    private async Task ProcessNotificationEventMessageAsync(string message, CancellationToken cancellationToken)
    {
        var notificationEvent = JsonSerializer.Deserialize<NotificationEventMessage>(message);

        if (notificationEvent == null)
        {
            throw new InvalidDataException("Notification event could not be deserialized.");
        }

        ValidateNotificationEventMessage(notificationEvent);

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ZenCareDbContext>();

        if (dbContext == null)
        {
            throw new InvalidOperationException("Worker database configuration is missing.");
        }

        var notificationExists = await dbContext.Notifications.AnyAsync(
            n => n.UserId == notificationEvent.UserId &&
                n.NotificationType == notificationEvent.EventKey,
            cancellationToken);

        if (notificationExists)
        {
            _logger.LogInformation("Notification event {EventKey} was already processed. Skipping duplicate.", notificationEvent.EventKey);
            return;
        }

        var userExists = await dbContext.Users.AnyAsync(
            u => u.Id == notificationEvent.UserId,
            cancellationToken);

        if (!userExists)
        {
            throw new InvalidDataException("Notification event references a user that does not exist.");
        }

        var sentAt = DateTime.UtcNow;

        dbContext.Notifications.Add(new Notification
        {
            UserId = notificationEvent.UserId,
            Title = notificationEvent.Title,
            Message = notificationEvent.Message,
            NotificationType = notificationEvent.EventKey,
            Status = NotificationStatus.Sent,
            SentAt = sentAt,
            CreatedAt = notificationEvent.OccurredAt
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Notification event {EventKey} was persisted.", notificationEvent.EventKey);
    }

    private static void ValidatePurchaseCreatedMessage(PurchaseCreatedMessage message)
    {
        if (message.PurchaseId <= 0)
        {
            throw new InvalidDataException("PurchaseId is required.");
        }

        if (string.IsNullOrWhiteSpace(message.PurchaseNumber))
        {
            throw new InvalidDataException("PurchaseNumber is required.");
        }

        if (message.UserId <= 0)
        {
            throw new InvalidDataException("UserId is required.");
        }

        if (message.TotalAmount <= 0)
        {
            throw new InvalidDataException("TotalAmount must be greater than zero.");
        }
    }

    private static void ValidateNotificationEventMessage(NotificationEventMessage message)
    {
        if (message.UserId <= 0)
        {
            throw new InvalidDataException("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(message.EventKey) || message.EventKey.Length > 50)
        {
            throw new InvalidDataException("EventKey is required and cannot exceed 50 characters.");
        }

        if (string.IsNullOrWhiteSpace(message.Title) || message.Title.Length > 150)
        {
            throw new InvalidDataException("Notification title is required and cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(message.Message) || message.Message.Length > 1000)
        {
            throw new InvalidDataException("Notification message is required and cannot exceed 1000 characters.");
        }

        if (message.OccurredAt == default)
        {
            throw new InvalidDataException("OccurredAt is required.");
        }
    }
}
