namespace FylumDeploy.ComposeBuilder;

internal interface IBuildService
{
    public Task<bool> BuildAsync(string commitHash, CancellationToken cancellationToken);
}
