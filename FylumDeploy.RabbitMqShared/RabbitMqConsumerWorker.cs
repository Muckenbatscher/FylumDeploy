using FylumDeploy.RabbitMqShared.MessagingModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FylumDeploy.RabbitMqShared;

public abstract class RabbitMqConsumerWorker<TMessage> : BackgroundService
{
    private readonly ILogger<RabbitMqConsumerWorker<TMessage>> _logger;
    private readonly IConnection _rabbitConnection;

    public RabbitMqConsumerWorker(ILogger<RabbitMqConsumerWorker<TMessage>> logger,
        IConnection rabbitConnection)
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection;
    }

    protected abstract string QueueName { get; }
    protected abstract Task<bool> ProcessMessageAsync(TMessage message);

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
                        var bodyBytes = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(bodyBytes);

                        _logger.LogInformation("Received {bytes} Bytes: {message}", bodyBytes.Length, message);

                        var success = false;
                        var deserializedMessage = JsonSerializer.Deserialize<TMessage>(message);
                        if (deserializedMessage is null)
                            _logger.LogError("Failed to deserialize message: {message}", message);
                        else
                            success = await ProcessMessageAsync(deserializedMessage);

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
