using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

namespace FylumWebhook;

public class FylumWebhookEventProcessor(ILogger<FylumWebhookEventProcessor> logger) : WebhookEventProcessor
{
    protected override async ValueTask ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action,
        CancellationToken cancellationToken = default)
    {
        switch (action)
        {
            case PullRequestActionValue.Opened:
                logger.LogInformation("pull request opened");
                await Task.Delay(1000, cancellationToken);
                break;
            default:
                logger.LogInformation("Some other pull request event");
                await Task.Delay(1000, cancellationToken);
                break;
        }
    }
}