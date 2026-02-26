using System.Text.Json.Serialization;

namespace FylumDeploy.RabbitMqShared.MessagingModels;

public record DeploymentResult(
    [property: JsonPropertyName("repo_owner")] string RepoOwner,
    [property: JsonPropertyName("repo_name")] string RepoName,
    [property: JsonPropertyName("commit_hash")] string CommitHash,
    [property: JsonPropertyName("success")] bool Success);
