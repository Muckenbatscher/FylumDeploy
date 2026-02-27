using FylumDeploy.ComposeBuilder.AspirePublish;
using FylumDeploy.ComposeBuilder.ContainerPublish;
using FylumDeploy.ComposeBuilder.RepoClone;

namespace FylumDeploy.ComposeBuilder;

internal class BuildService : IBuildService
{
    private readonly IRepoCloneService _repoCloneService;
    private readonly IContainerPublishService _containerPublishService;
    private readonly IAspirePublishService _aspirePublishService;

    public BuildService(
        IRepoCloneService repoCloneService,
        IContainerPublishService containerPublishService,
        IAspirePublishService aspirePublishService)
    {
        _repoCloneService = repoCloneService;
        _containerPublishService = containerPublishService;
        _aspirePublishService = aspirePublishService;
    }

    public async Task<bool> BuildAsync(string commitHash, CancellationToken cancellationToken)
    {
        var cloned = await _repoCloneService.CloneRepoAsync(commitHash, cancellationToken);
        if (!cloned)
            return false;

        var publishedContainers = await _containerPublishService.PublishContainersAsync(cancellationToken);
        if (!publishedContainers)
            return false;

        var aspireArtifactsPublished = await _aspirePublishService.PublishAspireArtifactsAsync(cancellationToken);
        if (!aspireArtifactsPublished)
            return false;

        return true;
    }
}
