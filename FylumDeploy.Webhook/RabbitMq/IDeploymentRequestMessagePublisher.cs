using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;

namespace FylumDeploy.Webhook.RabbitMq;

public interface IDeploymentRequestMessagePublisher : IRabbitMqPublisher<DeploymentRequest>
{
}