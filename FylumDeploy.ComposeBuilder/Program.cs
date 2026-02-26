using FylumDeploy.ComposeBuilder.ProcessExecution;
using FylumDeploy.ComposeBuilder.RepoClone;

namespace FylumDeploy.ComposeBuilder;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args)
            .AddServiceDefaults();

        builder.AddRabbitMQClient("rabbit");

        builder.Services.AddTransient<IDeploymentResultMessagePublisher, DeploymentResultMessagePublisher>();

        builder.Services.AddTransient<IProcessExecutionService, ProcessExecutionService>();
        builder.Services.AddTransient<IRepoCloneService, RepoCloneService>();

        builder.Services.AddHostedService<DeploymentRequestRabbitConsumerWorker>();

        builder.Build().Run();
    }
}
