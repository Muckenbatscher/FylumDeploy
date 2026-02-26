using FylumDeploy.ComposeBuilder.ContainerPublish;
using FylumDeploy.ComposeBuilder.RepoClone;

namespace FylumDeploy.ComposeBuilder;

internal class BuildService : IBuildService
{
    private readonly IRepoCloneService _repoCloneService;
    private readonly IContainerPublishService _containerPublishService;

    public BuildService(
        IRepoCloneService repoCloneService,
        IContainerPublishService containerPublishService)
    {
        _repoCloneService = repoCloneService;
        _containerPublishService = containerPublishService;
    }

    public async Task<bool> BuildAsync(string commitHash, CancellationToken cancellationToken)
    {
        var cloned = await _repoCloneService.CloneRepoAsync(commitHash, cancellationToken);
        if (!cloned)
            return false;

        var publishedContainers = await _containerPublishService.PublishContainersAsync(cancellationToken);
        if (!publishedContainers)
            return false;

        return true;
    }
}
