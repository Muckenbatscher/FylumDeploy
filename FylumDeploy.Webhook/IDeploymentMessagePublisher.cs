namespace FylumDeploy.Webhook;

public interface IDeploymentMessagePublisher
{
    Task PublishMessageAsync(string messageText, CancellationToken cancellationToken);
    Task PublishMessageAsync(string messageText);
}