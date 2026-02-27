namespace FylumDeploy.ComposeBuilder.AspirePublish;

internal interface IAspirePublishService
{
    Task<bool> PublishAspireArtifactsAsync(CancellationToken cancellationToken);
}
