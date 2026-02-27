using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.ComposeBuilder;

internal class DeploymentRequestRabbitConsumerWorker : RabbitMqConsumerWorker<DeploymentRequest>
{
    private readonly ILogger<DeploymentRequestRabbitConsumerWorker> _logger;
    private readonly IDeploymentService _deploymentService;
    private readonly IDeploymentResultMessagePublisher _resultPublisher;

    public DeploymentRequestRabbitConsumerWorker(
        ILogger<DeploymentRequestRabbitConsumerWorker> logger,
        IConnection rabbitConnection,
        IDeploymentResultMessagePublisher resultPublisher,
        IDeploymentService deploymentService)
        : base(logger, rabbitConnection)
    {
        _logger = logger;
        _resultPublisher = resultPublisher;
        _deploymentService = deploymentService;
    }

    protected override string QueueName => Queues.DeploymentRequests;

    protected override async Task<bool> ProcessMessageAsync(DeploymentRequest message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deploying commit '{commit}'.", message.CommitHash);

        var success = await _deploymentService.DeployAsync(message.CommitHash, cancellationToken);

        _logger.LogInformation("Deployment result: {resullt}", success ? "success" : "failure");

        var result = new DeploymentResult(
            RepoOwner: message.RepoOwner,
            RepoName: message.RepoName,
            CommitHash: message.CommitHash,
            Success: success);
        await _resultPublisher.PublishMessageAsync(result);
        _logger.LogInformation("Published result message.");

        return true;
    }
}
