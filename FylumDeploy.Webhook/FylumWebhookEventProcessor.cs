using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace FylumDeploy.Webhook;

public class FylumWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<FylumWebhookEventProcessor> _logger;

    public FylumWebhookEventProcessor(ILogger<FylumWebhookEventProcessor> logger)
    {
        _logger = logger;
    }

    protected override async ValueTask ProcessPushWebhookAsync(
        WebhookHeaders headers,
        PushEvent pushEvent,
        CancellationToken cancellationToken = default)
    {
        LogPushedRef(pushEvent.Ref);
    }

    private void LogPushedRef(string pushedRef)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Pushed to {Ref}", pushedRef);
    }
}