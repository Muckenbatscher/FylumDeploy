using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.ComposeBuilder;

internal class DeploymentRequestRabbitConsumerWorker : RabbitMqConsumerWorker<DeploymentRequest>
{
    private readonly ILogger<DeploymentRequestRabbitConsumerWorker> _logger;

    public DeploymentRequestRabbitConsumerWorker(
        ILogger<DeploymentRequestRabbitConsumerWorker> logger,
        IConnection rabbitConnection)
        : base(logger, rabbitConnection)
    {
        _logger = logger;
    }

    protected override string QueueName => Queues.DeploymentRequests;

    protected override async Task<bool> ProcessMessageAsync(DeploymentRequest message)
    {
        _logger.LogInformation("Deploying commit '{commit}'.", message.CommitHash);
        return true;
    }
}
