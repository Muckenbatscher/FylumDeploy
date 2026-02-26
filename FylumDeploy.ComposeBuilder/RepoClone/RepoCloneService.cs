using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.RepoClone;

internal class RepoCloneService : IRepoCloneService
{
    private const string RepoCloneUrl = "https://github.com/Muckenbatscher/Fylum.git";

    private readonly IProcessExecutionService _processExecutionService;

    public RepoCloneService(IProcessExecutionService processExecutionService)
    {
        _processExecutionService = processExecutionService;
    }

    public async Task<bool> CloneRepoAsync(string commitHash, string destinationPath, CancellationToken cancellationToken)
    {
        var command = $"git clone --depth 1 --revision {commitHash} {RepoCloneUrl} {destinationPath}";
        var processExecute = new ProcessExecute(command);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
}
