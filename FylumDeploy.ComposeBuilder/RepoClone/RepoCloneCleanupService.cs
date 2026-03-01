using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.RepoClone;

internal class RepoCloneCleanupService(
    ILogger<RepoCloneCleanupService> logger,
    IProcessExecutionService processExecutionService)
    : IRepoCloneCleanupService
{
    private readonly ILogger<RepoCloneCleanupService> _logger = logger;
    private readonly IProcessExecutionService _processExecutionService = processExecutionService;

    public async Task<bool> CleanupAsync(CancellationToken cancellationToken)
    {
        var command = $"rm -rf {Directories.BuildDirectory}";
        var processExecute = new ProcessExecute(command);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        if (result.WasSuccessful)
            _logger.LogInformation("Cleaned up the build directory.");
        else
        {
            _logger.LogError("Failed cleaning up the build directory.");
            _logger.LogError("Exit Code: {ExitCode}", result.ExitCode);
            _logger.LogError("Standard Output: {StandardOutput}", result.StandardOutput);
        }
        return result.WasSuccessful;
    }
}