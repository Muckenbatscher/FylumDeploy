namespace FylumDeploy.Webhook;

public interface IDeploymentRequestMessagePublisher
{
    Task PublishMessageAsync(string messageText, CancellationToken cancellationToken);
    Task PublishMessageAsync(string messageText);
}