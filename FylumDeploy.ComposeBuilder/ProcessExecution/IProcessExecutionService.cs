namespace FylumDeploy.ComposeBuilder.ProcessExecution;

internal interface IProcessExecutionService
{
    Task<ProcessExecutionResult> ExecuteProcessAsync(ProcessExecute processExecute, CancellationToken cancellationToken);
}