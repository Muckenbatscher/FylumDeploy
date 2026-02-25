using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using Octokit;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.GitHubStatusUpdate;

internal class DeploymentResultRabbitConsumerWorker : RabbitMqConsumerWorker<DeploymentResult>
{
    private readonly ILogger<DeploymentResultRabbitConsumerWorker> _logger;
    private readonly IGitHubClient _githubClient;

    public DeploymentResultRabbitConsumerWorker(
        ILogger<DeploymentResultRabbitConsumerWorker> logger,
        IConnection rabbitConnection,
        IGitHubClient githubClient) : base(logger, rabbitConnection)
    {
        _logger = logger;
        _githubClient = githubClient;
    }

    protected override string QueueName => Queues.DeploymentResults;

    protected override async Task<bool> ProcessMessageAsync(DeploymentResult deploymentResult)
    {
        var commitStatus = new NewCommitStatus
        {
            State = deploymentResult.Success ? CommitState.Success : CommitState.Failure,
            Description = deploymentResult.Success ? "Deployment succeeded" : "Deployment failed",
            Context = "Fylum Deploy DevOps-01"
        };
        var status = await _githubClient.Repository.Status.Create(
            deploymentResult.RepoOwner,
            deploymentResult.RepoName,
            deploymentResult.CommitHash,
            commitStatus);

        var statusText = deploymentResult.Success ? "success" : "failure";
        _logger.LogInformation("Created '{statusText}' status on GitHub. Id: {id}", statusText, status.Id);
        return true;
    }
}
