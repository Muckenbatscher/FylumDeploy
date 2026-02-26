namespace FylumDeploy.ComposeBuilder.ProcessExecution;

internal class ProcessExecute
{
    public string ExecutedFile { get; }
    public string Arguments { get; }
    public string WorkingDirectory { get; } = ".";

    public ProcessExecute(string executedFile, string arguments, string workingDirectory = ".")
    {
        ExecutedFile = executedFile;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
    }
    public ProcessExecute(string command, string workingDirectory = ".")
    {
        var commandParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());
        if (!commandParts.Any())
            throw new ArgumentException("Command cannot be empty.", nameof(command));

        ExecutedFile = commandParts.First();
        Arguments = string.Join(" ", commandParts.Skip(1));
        WorkingDirectory = workingDirectory;
    }
}
