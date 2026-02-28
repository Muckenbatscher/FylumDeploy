namespace FylumDeploy.ComposeBuilder.RepoClone;

internal interface IRepoCloneCleanupService
{
    Task<bool> CleanupAsync(CancellationToken cancellationToken);
}
