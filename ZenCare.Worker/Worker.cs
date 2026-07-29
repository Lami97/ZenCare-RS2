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
        try
        {
            await _rabbitMqService.InitializeAsync(stoppingToken);
            _logger.LogInformation("ZenCare Worker started. RabbitMQ infrastructure initialization completed.");
            await _rabbitMqService.RegisterConsumerAsync(RabbitMqService.PurchaseQueueName, ProcessPurchaseCreatedMessageAsync, stoppingToken);
            _logger.LogInformation("ZenCare Worker is consuming queue {QueueName}.", RabbitMqService.PurchaseQueueName);

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("ZenCare Worker is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ZenCare Worker encountered a RabbitMQ infrastructure error and will continue without processing messages.");
        }
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
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Notification created for purchase event {PurchaseId}.", purchaseCreated.PurchaseId);
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
}
