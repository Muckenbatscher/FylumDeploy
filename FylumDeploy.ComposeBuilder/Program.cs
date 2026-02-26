namespace FylumDeploy.ComposeBuilder;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args)
            .AddServiceDefaults();

        builder.AddRabbitMQClient("rabbit");

        builder.Services.AddTransient<IDeploymentResultMessagePublisher, DeploymentResultMessagePublisher>();

        builder.Services.AddHostedService<DeploymentRequestRabbitConsumerWorker>();

        builder.Build().Run();
    }
}
