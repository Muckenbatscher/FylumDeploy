using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using RabbitMQ.Client;

namespace FylumDeploy.ComposeBuilder;

public class DeploymentResultMessagePublisher : RabbitMqPublisher<DeploymentResult>, IDeploymentResultMessagePublisher
{
    public DeploymentResultMessagePublisher(
        ILogger<DeploymentResultMessagePublisher> logger,
        IConnection rabbitConnection)
        : base(logger, rabbitConnection)
    {
    }

    public override string QueueName => Queues.DeploymentResults;
}
