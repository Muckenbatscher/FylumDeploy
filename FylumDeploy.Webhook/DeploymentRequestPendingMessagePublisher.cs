using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using RabbitMQ.Client;

namespace FylumDeploy.Webhook;

public class DeploymentRequestPendingMessagePublisher : RabbitMqPublisher<DeploymentRequest>, IDeploymentRequestPendingMessagePublisher
{
    public DeploymentRequestPendingMessagePublisher(
        ILogger<DeploymentRequestPendingMessagePublisher> logger,
        IConnection rabbitConnection)
        : base(logger, rabbitConnection)
    {
    }

    public override string QueueName => Queues.DeploymentRequestsPending;
}
