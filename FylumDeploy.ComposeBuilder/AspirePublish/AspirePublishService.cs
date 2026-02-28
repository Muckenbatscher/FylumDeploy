using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.AspirePublish;

internal class AspirePublishService : IAspirePublishService
{
    private readonly IProcessExecutionService _processExecutionService;

    public AspirePublishService(IProcessExecutionService processExecutionService)
    {
        _processExecutionService = processExecutionService;
    }

    public async Task<bool> PublishAspireArtifactsAsync(CancellationToken cancellationToken)
    {
        var generated = await GenerateAspireArtifacts(cancellationToken);
        if (!generated)
            return false;

        bool copied = await CopyArtifactsToOutput(cancellationToken);

        return true;
    }

    private async Task<bool> GenerateAspireArtifacts(CancellationToken cancellationToken)
    {
        var command = $"aspire publish -o {Directories.IntermediatePublishDirectory} --non-interactive";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.BuildDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }

    private async Task<bool> CopyArtifactsToOutput(CancellationToken cancellationToken)
    {
        var copyCompose = CopyIntermediateArtifactToOutput("docker-compose.yaml", "podman-compose.yaml", cancellationToken);
        var copyBlankEnv = CopyIntermediateArtifactToOutput(".env", ".env.blank", cancellationToken);
        var results = await Task.WhenAll([copyCompose, copyBlankEnv]);
        return results.All(r => r);
    }

    private async Task<bool> CopyIntermediateArtifactToOutput(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
    {
        var sourceFilePath = Path.Combine(Directories.IntermediatePublishDirectory, sourceFileName);
        var targetFilePath = Path.Combine(Directories.OutputDirectory, targetFileName);

        var command = $"cp {sourceFilePath} {targetFilePath}";
        var processExecute = new ProcessExecute(command: command);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }

}
