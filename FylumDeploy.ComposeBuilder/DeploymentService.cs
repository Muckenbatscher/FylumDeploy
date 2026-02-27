namespace FylumDeploy.ComposeBuilder;

internal class DeploymentService : IDeploymentService
{
    private readonly IBuildService _buildService;

    public DeploymentService(IBuildService buildService)
    {
        _buildService = buildService;
    }

    public async Task<bool> DeployAsync(string commitHash, CancellationToken cancellationToken)
    {
        var builtToOutput = await _buildService.BuildAsync(commitHash, cancellationToken);
        if (!builtToOutput)
            return false;

        return true;
    }
}
