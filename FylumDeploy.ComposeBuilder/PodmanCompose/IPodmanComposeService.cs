namespace FylumDeploy.ComposeBuilder.PodmanCompose;

internal interface IPodmanComposeService
{
    Task<bool> ComposeDownAsync(CancellationToken cancellationToken);
    Task<bool> ComposeUpAsync(CancellationToken cancellationToken);
}
