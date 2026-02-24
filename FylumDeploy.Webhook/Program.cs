using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace FylumDeploy.Webhook;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args)
            .AddServiceDefaults();

        builder.AddRabbitMQClient("rabbit");

        builder.Services.AddSingleton<WebhookEventProcessor, FylumWebhookEventProcessor>();
        builder.Services.AddTransient<IDeploymentMessagePublisher, DeploymentMessagePublisher>();

        var app = builder.Build();

        app.UseRouting()
            .UseEndpoints(endpoints =>
            {
                var secret = builder.Configuration["GITHUB_SECRET"]!;
                endpoints.MapGitHubWebhooks(secret: secret);
            });

        app.Run();
    }
}
