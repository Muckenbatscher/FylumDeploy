internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<Projects.FylumDeploy_Webhook>("webhook")
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}