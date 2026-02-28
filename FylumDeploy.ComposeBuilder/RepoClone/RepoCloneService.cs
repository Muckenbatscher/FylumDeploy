using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.RepoClone;

internal class RepoCloneService(
    IProcessExecutionService processExecutionService,
    ILogger<RepoCloneService> logger)
    : IRepoCloneService
{
    private const string RepoCloneUrl = "https://github.com/Muckenbatscher/Fylum.git";

    private readonly ILogger<RepoCloneService> _logger = logger;
    private readonly IProcessExecutionService _processExecutionService = processExecutionService;

    public async Task<bool> CloneRepoAsync(string commitHash, CancellationToken cancellationToken)
    {
        var initCommand = $"git init {Directories.BuildDirectory}";
        var initProcess = new ProcessExecute(initCommand);
        var initResult = await _processExecutionService.ExecuteProcessAsync(initProcess, cancellationToken);
        if (!initResult.WasSuccessful)
        {
            _logger.LogError("Failed to initialize git repository at {directory}", Directories.BuildDirectory);
            return false;
        }
        _logger.LogInformation("Initialized git repository at {directory}", Directories.BuildDirectory);

        var remoteAddCommand = $"git remote add origin {RepoCloneUrl}";
        var remoteAddProcess = new ProcessExecute(remoteAddCommand, Directories.BuildDirectory);
        var remoteAddResul = await _processExecutionService.ExecuteProcessAsync(remoteAddProcess, cancellationToken);
        if (!remoteAddResul.WasSuccessful)
        {
            _logger.LogError("Failed to add remote origin {repoUrl} to git repository at {directory}",
                RepoCloneUrl, Directories.BuildDirectory);
            return false;
        }
        _logger.LogInformation("Added remote origin {repoUrl} to git repository at {directory}",
            RepoCloneUrl, Directories.BuildDirectory);

        var fetchCommand = $"git fetch --depth 1 origin {commitHash}";
        var fetchProcess = new ProcessExecute(fetchCommand, Directories.BuildDirectory);
        var fetchReuslt = await _processExecutionService.ExecuteProcessAsync(fetchProcess, cancellationToken);
        if (!fetchReuslt.WasSuccessful)
        {
            _logger.LogError("Failed to fetch commit {commitHash} from remote repository {repoUrl} in git repository at {directory}",
                commitHash, RepoCloneUrl, Directories.BuildDirectory);
            return false;
        }
        _logger.LogInformation("Fetched commit {commitHash} from remote repository {repoUrl} to set FETCH_HEAD",
            commitHash, RepoCloneUrl);

        var checkoutCommand = $"git checkout FETCH_HEAD";
        var checkoutProcess = new ProcessExecute(checkoutCommand, Directories.BuildDirectory);
        var checkoutResult = await _processExecutionService.ExecuteProcessAsync(checkoutProcess, cancellationToken);
        if (!checkoutResult.WasSuccessful)
        {
            _logger.LogError("Failed to checkout FETCH_HEAD in git repository at {directory}", Directories.BuildDirectory);
            return false;
        }
        _logger.LogInformation("Checked out FETCH_HEAD in git repository at {directory}", Directories.BuildDirectory);

        return true;
    }
}
