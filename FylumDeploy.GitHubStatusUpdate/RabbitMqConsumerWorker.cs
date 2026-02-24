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
    protected abstract bool ProcessMessage(string message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(queue: QueueName,
                durable: true, exclusive: false, autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received: {0}", message);

                var result = ProcessMessage(message);
                if (!result)
                {
                    _logger.LogError("Failed to process message: {0}", message);
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
                else
                {
                    _logger.LogInformation("Successfully processed message: {0}", message);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
            };
            await channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
