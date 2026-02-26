namespace FylumDeploy.ComposeBuilder.RepoClone;

public interface IRepoCloneService
{
    Task<bool> CloneRepoAsync(string commitHash, string destinamionPath, CancellationToken cancellationToken);
}
