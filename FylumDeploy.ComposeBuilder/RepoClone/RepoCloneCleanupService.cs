using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.RepoClone;

internal class RepoCloneCleanupService : IRepoCloneCleanupService
{
    private IProcessExecutionService _processExecutionService;

    public RepoCloneCleanupService(IProcessExecutionService processExecutionService)
    {
        _processExecutionService = processExecutionService;
    }

    public async Task<bool> CleanupAsync(CancellationToken cancellationToken)
    {
        var command = $"rm -rf {Directories.BuildDirectory}";
        var processExecute = new ProcessExecute(command);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
}