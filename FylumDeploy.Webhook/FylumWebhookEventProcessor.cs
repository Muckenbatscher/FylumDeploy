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
        _logger.LogInformation("pushed to {0}", pushEvent.Ref);
    }
}