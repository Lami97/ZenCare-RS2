using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ZenCare.Services.Interfaces;

namespace ZenCare.Services.Services
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private const int MaxProcessingAttempts = 5;
        public const string ExchangeName = "zencare.events";
        public const string AppointmentQueueName = "appointment-events";
        public const string PurchaseQueueName = "purchase-events";
        public const string PaymentQueueName = "payment-events";
        public const string NotificationQueueName = "notification-events";

        private static readonly IReadOnlyDictionary<string, string> QueueBindings = new Dictionary<string, string>
        {
            { AppointmentQueueName, "appointment" },
            { PurchaseQueueName, "purchase" },
            { PaymentQueueName, "payment" },
            { NotificationQueueName, "notification" }
        };

        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsConnected => _connection?.IsOpen == true && _channel?.IsOpen == true;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_channel?.IsOpen == true)
            {
                return;
            }

            var settings = GetSettings();

            if (settings == null)
            {
                _logger.LogWarning("RabbitMQ is not configured. Set RabbitMQ__Host, RabbitMQ__Port, RabbitMQ__Username and RabbitMQ__Password.");
                return;
            }

            try
            {
                await DisposeConnectionAsync();

                var factory = new ConnectionFactory
                {
                    HostName = settings.Host,
                    Port = settings.Port,
                    UserName = settings.Username,
                    Password = settings.Password
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await DeclareTopologyAsync(_channel, cancellationToken);

                _logger.LogInformation("RabbitMQ connection initialized. Exchange {ExchangeName} and queues declared.", ExchangeName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ connection could not be initialized. The application will continue without messaging.");
                await DisposeConnectionAsync();
            }
        }

        public async Task PublishAsync(string routingKey, string message, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken);

            if (_channel?.IsOpen != true)
            {
                _logger.LogWarning("RabbitMQ publish skipped because the channel is not available.");
                return;
            }

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }

        public async Task RegisterConsumerAsync(string queueName, Func<string, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken);

            if (_channel?.IsOpen != true)
            {
                throw new InvalidOperationException($"RabbitMQ consumer for queue {queueName} could not be registered because the channel is unavailable.");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                for (var attempt = 1; attempt <= MaxProcessingAttempts; attempt++)
                {
                    try
                    {
                        await handler(message, cancellationToken);
                        await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
                        return;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "RabbitMQ message on queue {QueueName} contains invalid JSON and will not be requeued.", queueName);
                        await _channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false, cancellationToken);
                        return;
                    }
                    catch (InvalidDataException ex)
                    {
                        _logger.LogWarning(ex, "RabbitMQ message on queue {QueueName} is invalid and will not be requeued.", queueName);
                        await _channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false, cancellationToken);
                        return;
                    }
                    catch (Exception ex) when (attempt < MaxProcessingAttempts)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                        _logger.LogWarning(
                            ex,
                            "RabbitMQ message processing failed on queue {QueueName}, attempt {Attempt}/{MaxAttempts}. Retrying in {DelaySeconds} seconds.",
                            queueName,
                            attempt,
                            MaxProcessingAttempts,
                            delay.TotalSeconds);
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "RabbitMQ message processing failed permanently on queue {QueueName} after {MaxAttempts} attempts.",
                            queueName,
                            MaxProcessingAttempts);
                        await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken);
                        return;
                    }
                }
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeConnectionAsync();
        }

        private async Task DeclareTopologyAsync(IChannel channel, CancellationToken cancellationToken)
        {
            await channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            foreach (var binding in QueueBindings)
            {
                await channel.QueueDeclareAsync(
                    queue: binding.Key,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: binding.Key,
                    exchange: ExchangeName,
                    routingKey: binding.Value,
                    cancellationToken: cancellationToken);
            }
        }

        private RabbitMqSettings? GetSettings()
        {
            var host = _configuration["RabbitMQ:Host"];
            var portValue = _configuration["RabbitMQ:Port"];
            var username = _configuration["RabbitMQ:Username"];
            var password = _configuration["RabbitMQ:Password"];

            if (string.IsNullOrWhiteSpace(host)
                || string.IsNullOrWhiteSpace(portValue)
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            if (!int.TryParse(portValue, out var port))
            {
                _logger.LogWarning("RabbitMQ port configuration is invalid.");
                return null;
            }

            return new RabbitMqSettings(host, port, username, password);
        }

        private async Task DisposeConnectionAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        private sealed record RabbitMqSettings(string Host, int Port, string Username, string Password);
    }
}
