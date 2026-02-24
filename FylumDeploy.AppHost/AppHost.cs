internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        var compose = builder.AddDockerComposeEnvironment("compose");

        var rabbitMq = builder.AddRabbitMQ("rabbit")
            .WithDataVolume("rabbitmq_data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithManagementPlugin();

        var githubApi = builder.AddExternalService("github", new Uri("https://api.github.com/"));

        var githubWebhookSecret = builder.AddParameter("github-webhook-secret", secret: true);
        var githubResponseAccessToken = builder.AddParameter("github-response-pat", secret: true);

        builder.AddProject<Projects.FylumDeploy_Webhook>("webhook")
            .WithEnvironment("GITHUB_SECRET", githubWebhookSecret)
            .WaitFor(rabbitMq)
            .WithReference(rabbitMq)
            .WithExternalHttpEndpoints();

        builder.AddProject<Projects.FylumDeploy_GitHubStatusUpdate>("status-update")
            .WithEnvironment("GITHUB_PAT", githubResponseAccessToken)
            .WaitFor(rabbitMq)
            .WithReference(rabbitMq)
            .WithReference(githubApi);

        builder.Build().Run();
    }
}