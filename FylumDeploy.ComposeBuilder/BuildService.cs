using FylumDeploy.ComposeBuilder.AspirePublish;
using FylumDeploy.ComposeBuilder.ContainerPublish;
using FylumDeploy.ComposeBuilder.RepoClone;

namespace FylumDeploy.ComposeBuilder;

internal class BuildService : IBuildService
{
    private readonly IRepoCloneCleanupService _repoCloneCleanupService;
    private readonly IRepoCloneService _repoCloneService;
    private readonly IContainerPublishService _containerPublishService;
    private readonly IAspirePublishService _aspirePublishService;

    public BuildService(
        IRepoCloneCleanupService repoCloneCleanupService,
        IRepoCloneService repoCloneService,
        IContainerPublishService containerPublishService,
        IAspirePublishService aspirePublishService)
    {
        _repoCloneCleanupService = repoCloneCleanupService;
        _repoCloneService = repoCloneService;
        _containerPublishService = containerPublishService;
        _aspirePublishService = aspirePublishService;
    }

    public async Task<bool> BuildAsync(string commitHash, CancellationToken cancellationToken)
    {
        await _repoCloneCleanupService.CleanupAsync(cancellationToken);
        var cloned = await _repoCloneService.CloneRepoAsync(commitHash, cancellationToken);
        if (!cloned)
            return false;

        var publishedContainers = await _containerPublishService.PublishContainersAsync(cancellationToken);
        if (!publishedContainers)
            return false;

        var aspireArtifactsPublished = await _aspirePublishService.PublishAspireArtifactsAsync(cancellationToken);
        if (!aspireArtifactsPublished)
            return false;

        await _repoCloneCleanupService.CleanupAsync(cancellationToken);

        return true;
    }
}
