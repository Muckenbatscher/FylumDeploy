namespace FylumDeploy.RabbitMqShared.MessagingModels;

public record DeploymentRequest(string RepoOwner, string RepoName, string BranchName, string CommitHash);
