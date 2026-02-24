namespace FylumDeploy.MessagingModels;

public record DeploymentResult(string RepoOwner, string RepoName, string CommitHash, bool Success);
