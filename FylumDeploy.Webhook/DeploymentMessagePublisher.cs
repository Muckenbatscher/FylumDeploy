using RabbitMQ.Client;
using System.Text;

namespace FylumDeploy.Webhook;

public class DeploymentMessagePublisher : IDeploymentMessagePublisher
{
    private readonly ILogger<DeploymentMessagePublisher> _logger;
    private readonly IConnection _rabbitConnection;

    public DeploymentMessagePublisher(ILogger<DeploymentMessagePublisher> logger,
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

        await channel.QueueDeclareAsync(queue: "deployments", durable: true,
            exclusive: false, autoDelete: false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(messageText);

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: string.Empty, body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Sent: {messageText}", messageText);
    }
}
