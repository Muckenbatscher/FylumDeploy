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

    public async Task PublishMessageAsync(string messageText) =>
        await PublishMessageAsync(messageText, CancellationToken.None);

    public async Task PublishMessageAsync(string messageText, CancellationToken cancellationToken)
    {
        using var channel = await _rabbitConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: Routes.DeploymentRequests, durable: true,
            exclusive: false, autoDelete: false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(messageText);

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: Routes.DeploymentRequests, body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Sent: {messageText}", messageText);
    }
}
