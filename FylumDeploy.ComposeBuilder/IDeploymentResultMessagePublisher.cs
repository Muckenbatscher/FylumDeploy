using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;

namespace FylumDeploy.ComposeBuilder;

public interface IDeploymentResultMessagePublisher : IRabbitMqPublisher<DeploymentResult>
{
}