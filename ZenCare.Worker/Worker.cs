using ZenCare.Services.Interfaces;

namespace ZenCare.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitMqService _rabbitMqService;

    public Worker(ILogger<Worker> logger, IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _rabbitMqService.InitializeAsync(stoppingToken);
            _logger.LogInformation("ZenCare Worker started. RabbitMQ infrastructure initialization completed.");

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
}
