using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;

namespace FylumDeploy.Webhook;

public interface IDeploymentRequestPendingMessagePublisher : IRabbitMqPublisher<DeploymentRequest>
{
}