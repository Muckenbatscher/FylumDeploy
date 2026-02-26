using System.Text.Json.Serialization;

namespace FylumDeploy.RabbitMqShared.MessagingModels;

public record DeploymentRequest(
    [property: JsonPropertyName("repo_owner")] string RepoOwner,
    [property: JsonPropertyName("repo_name")] string RepoName,
    [property: JsonPropertyName("branch_name")] string BranchName,
    [property: JsonPropertyName("commit_hash")] string CommitHash);
