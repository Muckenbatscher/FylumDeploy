using FylumDeploy.ComposeBuilder.PodmanCompose;

namespace FylumDeploy.ComposeBuilder;

internal class DeploymentService : IDeploymentService
{
    private readonly IBuildService _buildService;
    private readonly IPodmanComposeService _podmanComposeService;

    public DeploymentService(
        IBuildService buildService,
        IPodmanComposeService podmanComposeService)
    {
        _buildService = buildService;
        _podmanComposeService = podmanComposeService;
    }

    public async Task<bool> DeployAsync(string commitHash, CancellationToken cancellationToken)
    {
        var composedDown = await _podmanComposeService.ComposeDownAsync(cancellationToken);
        if (!composedDown)
            return false;

        var builtToOutput = await _buildService.BuildAsync(commitHash, cancellationToken);
        if (!builtToOutput)
            return false;

        var composedUp = await _podmanComposeService.ComposeUpAsync(cancellationToken);
        if (!composedUp)
            return false;

        return true;
    }
}
