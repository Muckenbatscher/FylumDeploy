using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.AspirePublish;

internal class AspirePublishService(
    IProcessExecutionService processExecutionService,
    ILogger<AspirePublishService> logger)
    : IAspirePublishService
{
    private readonly ILogger<AspirePublishService> _logger = logger;
    private readonly IProcessExecutionService _processExecutionService = processExecutionService;

    public async Task<bool> PublishAspireArtifactsAsync(CancellationToken cancellationToken)
    {
        var upToDate = await EnsureAspireUpdated(cancellationToken);
        if (!upToDate)
            return false;

        var generated = await GenerateAspireArtifacts(cancellationToken);
        if (!generated)
            return false;

        var copied = await CopyArtifactsToOutput(cancellationToken);
        return copied;
    }

    private async Task<bool> EnsureAspireUpdated(CancellationToken cancellationToken)
    {
        var updateSelfCommand = "aspire update --self --non-interactive --yes";
        var updateSelfProcessExecute = new ProcessExecute(command: updateSelfCommand);
        var updateSelfResult = await _processExecutionService.ExecuteProcessAsync(updateSelfProcessExecute, cancellationToken);
        if (!updateSelfResult.WasSuccessful)
            return false;

        var setupBundleCommand = "aspire setup --non-interactive";
        var setupBundleProcessExecute = new ProcessExecute(command:  setupBundleCommand);
        var setupBundleResult = await _processExecutionService.ExecuteProcessAsync(setupBundleProcessExecute, cancellationToken);
        return setupBundleResult.WasSuccessful;
    }

    private async Task<bool> GenerateAspireArtifacts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Aspire artifacts into intermediate directory...");
        var command = $"aspire publish -o {Directories.IntermediatePublishDirectory} --non-interactive";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.BuildDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        if (result.WasSuccessful)
            _logger.LogInformation("Aspire artifacts generated successfully.");
        else
            _logger.LogError("Failed to generate Aspire artifacts.");

        return result.WasSuccessful;
    }

    private async Task<bool> CopyArtifactsToOutput(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Copying artifacts from intermediate directory to publish directory...");
        var copyCompose = CopyIntermediateArtifactToOutput("docker-compose.yaml", "podman-compose.yaml", cancellationToken);
        var copyBlankEnv = CopyIntermediateArtifactToOutput(".env", ".env.blank", cancellationToken);
        var results = await Task.WhenAll([copyCompose, copyBlankEnv]);
        var allCopied = results.All(r => r);

        if (allCopied)
            _logger.LogInformation("All artifacts copied successfully.");
        else
            _logger.LogError("Failed to copy all artifacts.");
        return allCopied;
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
