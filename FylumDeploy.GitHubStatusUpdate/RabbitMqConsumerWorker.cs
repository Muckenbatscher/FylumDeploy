using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FylumDeploy.GitHubStatusUpdate;

internal abstract class RabbitMqConsumerWorker : BackgroundService
{
    private readonly ILogger<RabbitMqConsumerWorker> _logger;
    private readonly IConnection _rabbitConnection;

    public RabbitMqConsumerWorker(ILogger<RabbitMqConsumerWorker> logger,
        IConnection rabbitConnection)
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection;
    }

    protected abstract string QueueName { get; }
    protected abstract Task<bool> ProcessMessageAsync(string message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Queue '{QueueName}' waiting for connection...", QueueName);

                using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: stoppingToken);

                await channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1,
                    global: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        _logger.LogInformation("Received: {0}", message);

                        var success = await ProcessMessageAsync(message);

                        if (success)
                            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                        else
                            await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Critical error while trying to process the message.");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Consumer for queue '{QueueName}' active.", QueueName);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in the consumer-loop. Restarting in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
