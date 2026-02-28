using System.Diagnostics;
using System.Text;

namespace FylumDeploy.ComposeBuilder.ProcessExecution;

internal class ProcessExecutionService(ILogger<ProcessExecutionService> logger)
    : IProcessExecutionService
{
    private readonly ILogger<ProcessExecutionService> _logger = logger;

    public async Task<ProcessExecutionResult> ExecuteProcessAsync(ProcessExecute processExecute, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = processExecute.ExecutedFile,
            Arguments = processExecute.Arguments,
            WorkingDirectory = processExecute.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        using var process = new Process { StartInfo = startInfo };

        // We use TaskCompletionSource to signal when the streams are actually closed
        var outputCloseSignal = new TaskCompletionSource<bool>();
        var errorCloseSignal = new TaskCompletionSource<bool>();

        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data == null)
                outputCloseSignal.SetResult(true);
            else
                outputBuilder.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (s, e) =>
        {
            if (e.Data == null)
                errorCloseSignal.SetResult(true);
            else
                errorBuilder.AppendLine(e.Data);
        };

        process.Start();

        // Start the asynchronous read of the streams
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken);
            await Task.WhenAll(outputCloseSignal.Task, errorCloseSignal.Task)
                .WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // If canceled, ensure the process is terminated
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);

            throw; // Re-throw to let the caller know it was canceled
        }
        var result = new ProcessExecutionResult(
            process.ExitCode,
            outputBuilder.ToString().Trim(),
            errorBuilder.ToString().Trim()
        );

        LogErrorResult(processExecute, result);

        return result;
    }

    private void LogErrorResult(ProcessExecute processExecute, ProcessExecutionResult result)
    {
        if (result.WasSuccessful)
            return;

        var command = $"{processExecute.ExecutedFile} {processExecute.Arguments}".Trim();
        _logger.LogError("Process execution failed for command: '{Command}' in working directory: '{workingDirectory}'",
            command, processExecute.WorkingDirectory);

        _logger.LogError("Process execution failed. Exit Code: {ExitCode}, Standard Error: {StandardError}, Standard Output: {StandardOutput}",
            result.ExitCode, result.StandardError, result.StandardOutput);
    }
}
