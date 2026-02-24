using Octokit;

namespace FylumDeploy.GitHubStatusUpdate;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args)
            .AddServiceDefaults();

        builder.AddRabbitMQClient("rabbit");

        var client = new GitHubClient(
            productInformation: new ProductHeaderValue("deploy-status"), 
            baseAddress: new Uri("https://github"));
    }
}
