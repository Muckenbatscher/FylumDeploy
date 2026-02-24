using FylumDeploy.MessagingModels;
using RabbitMQ.Client;
using System.Text;

namespace FylumDeploy.Webhook;

public class DeploymentRequestMessagePublisher : IDeploymentRequestMessagePublisher
{
    private readonly ILogger<DeploymentRequestMessagePublisher> _logger;
    private readonly IConnection _rabbitConnection;

    public DeploymentRequestMessagePublisher(ILogger<DeploymentRequestMessagePublisher> logger,
        IConnection rabbitConnection)
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection;
    }

    public async Task PublishMessageAsync(DeploymentRequest deploymentRequest) =>
        await PublishMessageAsync(deploymentRequest, CancellationToken.None);

    public async Task PublishMessageAsync(DeploymentRequest deploymentRequest, CancellationToken cancellationToken)
    {
        await SendDeploymentRequestToQueue(deploymentRequest, Queues.DeploymentRequests, cancellationToken);
        await SendDeploymentRequestToQueue(deploymentRequest, Queues.DeploymentRequestsPending, cancellationToken);
    }

    private async Task SendDeploymentRequestToQueue(DeploymentRequest deploymentRequest, string queueName, CancellationToken cancellationToken)
    {
        using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: queueName, durable: true,
                    exclusive: false, autoDelete: false,
                    cancellationToken: cancellationToken);

        var messageText = System.Text.Json.JsonSerializer.Serialize(deploymentRequest);
        var body = Encoding.UTF8.GetBytes(messageText);

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: queueName, body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published deployment request to queue: {queueName}", queueName);
    }
}
