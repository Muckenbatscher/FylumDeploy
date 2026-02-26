namespace FylumDeploy.ComposeBuilder.ProcessExecution;

internal class ProcessExecutionResult
{
    public ProcessExecutionResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }

    public int ExitCode { get; }
    public bool WasSuccessful => ExitCode == 0;

    public string StandardOutput { get; } = string.Empty;
    public string StandardError { get; } = string.Empty;
}
