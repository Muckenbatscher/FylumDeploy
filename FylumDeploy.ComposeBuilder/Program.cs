using FylumDeploy.ComposeBuilder.AspirePublish;
using FylumDeploy.ComposeBuilder.ContainerPublish;
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
        builder.Services.AddTransient<IContainerPublishService, ContainerPublishService>();
        builder.Services.AddTransient<IAspirePublishService, AspirePublishService>();

        builder.Services.AddHostedService<DeploymentRequestRabbitConsumerWorker>();

        builder.Build().Run();
    }
}
