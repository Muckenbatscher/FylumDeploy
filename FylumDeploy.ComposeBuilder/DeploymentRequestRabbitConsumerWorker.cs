using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.ComposeBuilder;

internal class DeploymentRequestRabbitConsumerWorker : RabbitMqConsumerWorker<DeploymentRequest>
{
    private readonly ILogger<DeploymentRequestRabbitConsumerWorker> _logger;
    private readonly IDeploymentResultMessagePublisher _resultPublisher;

    public DeploymentRequestRabbitConsumerWorker(
        ILogger<DeploymentRequestRabbitConsumerWorker> logger,
        IConnection rabbitConnection,
        IDeploymentResultMessagePublisher resultPublisher)
        : base(logger, rabbitConnection)
    {
        _logger = logger;
        _resultPublisher = resultPublisher;
    }

    protected override string QueueName => Queues.DeploymentRequests;

    protected override async Task<bool> ProcessMessageAsync(DeploymentRequest message)
    {
        _logger.LogInformation("Deploying commit '{commit}'.", message.CommitHash);

        // TODO: actual deployment
        await Task.Delay(TimeSpan.FromSeconds(30));
        var success = true;
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
