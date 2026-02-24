using FylumDeploy.MessagingModels;
using Octokit;
using System.Text.Json;
using IConnection = RabbitMQ.Client.IConnection;

namespace FylumDeploy.GitHubStatusUpdate;

internal class DeploymentResultRabbitConsumerWorker : RabbitMqConsumerWorker
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

    protected override string QueueName => Routes.DeploymentResults;

    protected override async Task<bool> ProcessMessageAsync(string message)
    {
        var deploymentResult = JsonSerializer.Deserialize<DeploymentResult>(message);
        if (deploymentResult is null)
        {
            _logger.LogError("Failed to deserialize message: {message}", message);
            return false;
        }

        var commitStatus = new NewCommitStatus
        {
            State = deploymentResult.Success ? CommitState.Success : CommitState.Failure,
            Description = deploymentResult.Success ? "Deployment succeeded" : "Deployment failed",
            Context = "Fylum Deploy DevOps-01"
        };
        await _githubClient.Repository.Status.Create(
            deploymentResult.RepoOwner,
            deploymentResult.RepoName,
            deploymentResult.CommitHash,
            commitStatus);
        return true;
    }
}
