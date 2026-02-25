using FylumDeploy.RabbitMqShared;
using FylumDeploy.RabbitMqShared.MessagingModels;
using RabbitMQ.Client;

namespace FylumDeploy.Webhook;

public class DeploymentRequestMessagePublisher : RabbitMqPublisher<DeploymentRequest>, IDeploymentRequestMessagePublisher
{
    public DeploymentRequestMessagePublisher(
        ILogger<DeploymentRequestMessagePublisher> logger,
        IConnection rabbitConnection)
        : base(logger, rabbitConnection)
    {
    }

    public override string QueueName => Queues.DeploymentRequests;
}
