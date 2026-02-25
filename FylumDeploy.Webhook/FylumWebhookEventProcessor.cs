using FylumDeploy.RabbitMqShared.MessagingModels;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace FylumDeploy.Webhook;

public class FylumWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<FylumWebhookEventProcessor> _logger;
    private readonly IDeploymentRequestMessagePublisher _requestMessagePublisher;
    private readonly IDeploymentRequestPendingMessagePublisher _requestPendingMessagePublisher;

    public FylumWebhookEventProcessor(
        ILogger<FylumWebhookEventProcessor> logger,
        IDeploymentRequestMessagePublisher messagePublisher,
        IDeploymentRequestPendingMessagePublisher requestPendingMessagePublisher)
    {
        _logger = logger;
        _requestMessagePublisher = messagePublisher;
        _requestPendingMessagePublisher = requestPendingMessagePublisher;
    }

    protected override async ValueTask ProcessPushWebhookAsync(
        WebhookHeaders headers,
        PushEvent pushEvent,
        CancellationToken cancellationToken = default)
    {
        var commit = pushEvent.After;
        var branch = pushEvent.Ref;
        LogPushedRef(commit, branch);

        var repository = pushEvent.Repository;
        if (repository is null)
            return;

        var deployRequest = new DeploymentRequest(
            RepoOwner: repository.Owner.Login,
            RepoName: repository.Name,
            BranchName: branch,
            CommitHash: commit);
        await _requestMessagePublisher.PublishMessageAsync(deployRequest, cancellationToken);
        await _requestPendingMessagePublisher.PublishMessageAsync(deployRequest, cancellationToken);
        LogMessagePublished();
    }

    private void LogPushedRef(string commitHash, string branch)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Pushed {commitHash} to branch {branch}", commitHash, branch);
    }

    private void LogMessagePublished()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Published message");
    }
}