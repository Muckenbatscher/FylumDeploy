using FylumDeploy.MessagingModels;

namespace FylumDeploy.Webhook;

public interface IDeploymentRequestMessagePublisher
{
    Task PublishMessageAsync(DeploymentRequest deploymentRequest, CancellationToken cancellationToken);
    Task PublishMessageAsync(DeploymentRequest deploymentRequest);
}