internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        var compose = builder.AddDockerComposeEnvironment("compose");

        var githubWebhookSecret = builder.AddParameter("GithubWebhookSecret", secret: true);
        builder.AddProject<Projects.FylumDeploy_Webhook>("webhook")
            .WithExternalHttpEndpoints()
            .WithEnvironment("GITHUB_SECRET", githubWebhookSecret);

        builder.Build().Run();
    }
}