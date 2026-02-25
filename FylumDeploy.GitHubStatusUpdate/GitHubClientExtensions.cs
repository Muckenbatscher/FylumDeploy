using Octokit;
using Octokit.Internal;

namespace FylumDeploy.GitHubStatusUpdate;

internal static class GitHubClientExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGitHubClient(string accessToken)
        {
            return services
                .AddConnection(accessToken)
                .AddClient();
        }

        private IServiceCollection AddConnection(string accessToken)
        {
            return services
                .AddSingleton<IConnection>(sp =>
                {
                    var handlerFactory = sp.GetRequiredService<IHttpMessageHandlerFactory>();
                    var httpClientAdapter = new HttpClientAdapter(() => handlerFactory.CreateHandler("GitHub"));

                    return new Connection(
                        new ProductHeaderValue("deploy-status"),
                        new Uri("https://github"),
                        new InMemoryCredentialStore(new Credentials(accessToken)),
                        httpClientAdapter,
                        new SimpleJsonSerializer()
                    );
                });
        }
        private IServiceCollection AddClient()
        {
            return services
                .AddSingleton<IGitHubClient, GitHubClient>(sp =>
                {
                    var connection = sp.GetRequiredService<IConnection>();
                    return new GitHubClient(connection);
                });
        }
    }
}
