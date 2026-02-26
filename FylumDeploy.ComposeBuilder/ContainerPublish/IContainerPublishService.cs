namespace FylumDeploy.ComposeBuilder.ContainerPublish;

internal interface IContainerPublishService
{
    public Task<bool> PublishContainersAsync(CancellationToken cancellationToken);
}
