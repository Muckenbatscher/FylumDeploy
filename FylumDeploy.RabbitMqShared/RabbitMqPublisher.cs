using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace FylumDeploy.RabbitMqShared;

public abstract class RabbitMqPublisher<TMessage> : IRabbitMqPublisher<TMessage>
{
    private readonly ILogger<RabbitMqPublisher<TMessage>> _logger;
    private readonly IConnection _rabbitConnection;

    public RabbitMqPublisher(
        ILogger<RabbitMqPublisher<TMessage>> logger,
        IConnection rabbitConnection)
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection;
    }

    public abstract string QueueName { get; }

    public async Task PublishMessageAsync(TMessage message) =>
        await PublishMessageAsync(message, CancellationToken.None);

    public async Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: QueueName, durable: true,
                    exclusive: false, autoDelete: false,
                    cancellationToken: cancellationToken);

        var messageText = System.Text.Json.JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageText);

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: QueueName, body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published deployment request to queue: {queueName}", QueueName);
    }
}
