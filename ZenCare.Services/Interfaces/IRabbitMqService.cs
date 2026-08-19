namespace ZenCare.Services.Interfaces
{
    public interface IRabbitMqService
    {
        bool IsConnected { get; }

        Task InitializeAsync(CancellationToken cancellationToken = default);

        Task PublishAsync(string routingKey, string message, CancellationToken cancellationToken = default);

        Task RegisterConsumerAsync(string queueName, Func<string, CancellationToken, Task> handler, CancellationToken cancellationToken = default);
    }
}
