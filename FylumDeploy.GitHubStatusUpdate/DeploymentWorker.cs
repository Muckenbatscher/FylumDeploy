using RabbitMQ.Client;

namespace FylumDeploy.GitHubStatusUpdate;

internal class DeploymentWorker : RabbitMqConsumerWorker
{
    private readonly ILogger<DeploymentWorker> _logger;

    public DeploymentWorker(ILogger<DeploymentWorker> logger,
        IConnection rabbitConnection) : base(logger, rabbitConnection)
    {
        _logger = logger;
    }

    protected override string QueueName => "deployments";

    protected override bool ProcessMessage(string message)
    {
        _logger.LogInformation("Received in Worker: {0}", message);
        return true;
    }
}
