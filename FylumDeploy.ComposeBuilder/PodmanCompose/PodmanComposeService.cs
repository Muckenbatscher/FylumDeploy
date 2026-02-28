using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.PodmanCompose;

internal class PodmanComposeService(
    ILogger<PodmanComposeService> logger,
    IProcessExecutionService processExecutionService)
    : IPodmanComposeService
{
    private readonly ILogger<PodmanComposeService> _logger = logger;
    private readonly IProcessExecutionService _processExecutionService = processExecutionService;

    public async Task<bool> ComposeDownAsync(CancellationToken cancellationToken)
    {
        string workingDirectory = Directories.OutputDirectory;
        string composeFilePath = Path.Combine(workingDirectory, "podman-compose.yaml");

        // 1. Prüfen, ob dies ein Update oder das erste Deployment ist
        if (!File.Exists(composeFilePath))
        {
            _logger.LogWarning("No previous deployment found. Skipping 'compose down'");
            return true;
        }

        _logger.LogInformation("Existing deployment found. Trying to stop old containers...");

        var command = $"podman compose down";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.OutputDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
    public async Task<bool> ComposeUpAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Composing up new containers...");
        var command = $"podman compose up -d";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.OutputDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
}
