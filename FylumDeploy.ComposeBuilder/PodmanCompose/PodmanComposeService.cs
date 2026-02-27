using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.PodmanCompose;

internal class PodmanComposeService : IPodmanComposeService
{
    private readonly IProcessExecutionService _processExecutionService;

    public PodmanComposeService(IProcessExecutionService processExecutionService)
    {
        _processExecutionService = processExecutionService;
    }

    public async Task<bool> ComposeDownAsync(CancellationToken cancellationToken)
    {
        var command = $"podman compose down";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.OutputDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
    public async Task<bool> ComposeUpAsync(CancellationToken cancellationToken)
    {
        var command = $"podman compose up -d";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.OutputDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
}
