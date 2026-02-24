using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace FylumDeploy.Webhook;

public class FylumWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<FylumWebhookEventProcessor> _logger;
    private readonly IDeploymentMessagePublisher _messagePublisher;

    public FylumWebhookEventProcessor(
        ILogger<FylumWebhookEventProcessor> logger,
        IDeploymentMessagePublisher messagePublisher)
    {
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    protected override async ValueTask ProcessPushWebhookAsync(
        WebhookHeaders headers,
        PushEvent pushEvent,
        CancellationToken cancellationToken = default)
    {
        LogPushedRef(pushEvent.Ref);
        await _messagePublisher.PublishMessageAsync(pushEvent.Ref, cancellationToken);
        LogMessagePublished();
    }

    private void LogPushedRef(string pushedRef)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Pushed to {Ref}", pushedRef);
    }

    private void LogMessagePublished()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Published message");
    }
}