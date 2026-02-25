namespace FylumDeploy.RabbitMqShared.MessagingModels;

public record DeploymentResult(string RepoOwner, string RepoName, string CommitHash, bool Success);
