namespace FylumDeploy.GitHubStatusUpdate;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args)
            .AddServiceDefaults();

        builder.AddRabbitMQClient("rabbit");

        var token = builder.Configuration["GITHUB_PAT"]!;
        builder.Services.AddGitHubClient(token);

        builder.Services.AddHostedService<DeploymentResultRabbitConsumerWorker>();

        builder.Build().Run();
    }
}
