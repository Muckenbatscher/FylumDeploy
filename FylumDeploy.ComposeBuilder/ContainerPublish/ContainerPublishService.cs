using FylumDeploy.ComposeBuilder.ProcessExecution;

namespace FylumDeploy.ComposeBuilder.ContainerPublish;

internal class ContainerPublishService(
    IProcessExecutionService processExecutionService,
    ILogger<ContainerPublishService> logger)
    : IContainerPublishService
{
    private const string BuildConfiguration = "Release";

    private readonly ILogger<ContainerPublishService> _logger = logger;
    private readonly IProcessExecutionService _processExecutionService = processExecutionService;

    public async Task<bool> PublishContainersAsync(CancellationToken cancellationToken)
    {
        var buildSuccessful = await BuildDotnetSolutionAsync(cancellationToken);
        if (!buildSuccessful)
            return false;

        var publishTasks = new List<Task<bool>>
        {
            PublishDotnetProjectContainerAsync("Fylum.Api", ImageNames.ApiImageName, cancellationToken),
            PublishDotnetProjectContainerAsync("Fylum.Web", ImageNames.WebImageName, cancellationToken),
            PublishDotnetProjectContainerAsync("Fylum.Migrations.Api", ImageNames.MigrationApiImageName, cancellationToken),
            PublishDotnetProjectContainerAsync("Fylum.Migrations.Web", ImageNames.MigrationWebImageName, cancellationToken),
        };
        bool[] results = await Task.WhenAll(publishTasks);
        bool allSuccessful = results.All(r => r);
        return allSuccessful;
    }

    private async Task<bool> BuildDotnetSolutionAsync(CancellationToken cancellationToken)
    {
        var command = $"dotnet build -c {BuildConfiguration}";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.BuildDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        return result.WasSuccessful;
    }
    private async Task<bool> PublishDotnetProjectContainerAsync(string projectName, string imageName,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Building '{project}' image...", projectName);
        string projectPath = Path.Combine("./", "Source", projectName);
        var command = $"dotnet publish {projectPath} --no-build -c Release -t PublishContainer -p ContainerRepository={imageName}";
        var processExecute = new ProcessExecute(command: command, workingDirectory: Directories.BuildDirectory);
        var result = await _processExecutionService.ExecuteProcessAsync(processExecute, cancellationToken);
        if (result.WasSuccessful)
            _logger.LogInformation("Finished building '{project}' image.", projectName);
        else
            _logger.LogError("Failed to build '{project}' image.", projectName);

        return result.WasSuccessful;
    }
}
