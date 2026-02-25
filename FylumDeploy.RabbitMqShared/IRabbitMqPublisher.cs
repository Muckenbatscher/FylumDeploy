namespace FylumDeploy.RabbitMqShared;

public interface IRabbitMqPublisher<TMessage>
{
    Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken);
    Task PublishMessageAsync(TMessage message);
}
