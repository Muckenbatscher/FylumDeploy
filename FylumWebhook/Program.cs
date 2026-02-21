using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

namespace FylumWebhook;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<WebhookEventProcessor, FylumWebhookEventProcessor>();

        var app = builder.Build();

        app.UseRouting()
            .UseEndpoints(endpoints =>
            {
                var secret = builder.Configuration["GitHubWebhookSecret"];
                endpoints.MapGitHubWebhooks();
            });

        app.Run();
    }
}
