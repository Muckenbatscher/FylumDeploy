using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using Octokit;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.GitHubStatusUpdate;

internal class PendingDeploymentRequestRabbitConsumerWorker : RabbitMqConsumerWorker<DeploymentRequest>
{
    private readonly ILogger<PendingDeploymentRequestRabbitConsumerWorker> _logger;
    private readonly IGitHubClient _githubClient;

    public PendingDeploymentRequestRabbitConsumerWorker(
        ILogger<PendingDeploymentRequestRabbitConsumerWorker> logger,
        IConnection rabbitConnection,
        IGitHubClient githubClient) : base(logger, rabbitConnection)
    {
        _logger = logger;
        _githubClient = githubClient;
    }

    protected override string QueueName => Queues.DeploymentRequestsPending;

    protected override async Task<bool> ProcessMessageAsync(DeploymentRequest deploymentRequest)
    {
        var commitStatus = new NewCommitStatus
        {
            State = CommitState.Pending,
            Description = "Deployment pending",
            Context = "Fylum Deploy DevOps-01"
        };
        var status = await _githubClient.Repository.Status.Create(
            deploymentRequest.RepoOwner,
            deploymentRequest.RepoName,
            deploymentRequest.CommitHash,
            commitStatus);
        _logger.LogInformation("Created pending status on GitHub. Id: {id}", status.Id);
        return true;
    }
}
