namespace FylumDeploy.ComposeBuilder;

internal interface IDeploymentService
{
    Task<bool> DeployAsync(string commitHash, CancellationToken cancellationToken);
}
