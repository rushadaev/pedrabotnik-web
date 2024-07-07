using Newtonsoft.Json;
using Pedrabotnik.Models.Assistant.Enums;

namespace Pedrabotnik.Models.Assistant;

public class AssistantRun
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("assistant_id")]
    public string AssistantId { get; set; }

    [JsonProperty("thread_id")]
    public string ThreadId { get; set; }

    [JsonProperty("status")]
    public AssistantRunStatus Status { get; set; }

    [JsonProperty("started_at")]
    public long? StartedAt { get; set; }

    [JsonProperty("expires_at")]
    public long? ExpiresAt { get; set; }

    [JsonProperty("cancelled_at")]
    public long? CancelledAt { get; set; }

    [JsonProperty("failed_at")]
    public long? FailedAt { get; set; }

    [JsonProperty("completed_at")]
    public long? CompletedAt { get; set; }

    [JsonProperty("last_error")]
    public string? LastError { get; set; }

    [JsonProperty("model")]
    public string? Model { get; set; }

    [JsonProperty("instructions")]
    public string? Instructions { get; set; }

    [JsonProperty("incomplete_details")]
    public string? IncompleteDetails { get; set; }

    [JsonProperty("tools")]
    public List<AssistantRunTool>? Tools { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonProperty("usage")]
    public string? Usage { get; set; }

    [JsonProperty("temperature")]
    public double? Temperature { get; set; }

    [JsonProperty("top_p")]
    public double? TopP { get; set; }

    [JsonProperty("max_prompt_tokens")]
    public int? MaxPromptTokens { get; set; }

    [JsonProperty("max_completion_tokens")]
    public int? MaxCompletionTokens { get; set; }

    [JsonProperty("truncation_strategy")]
    public AssistantRunTruncationStrategy? TruncationStrategy { get; set; }

    [JsonProperty("response_format")]
    public string? ResponseFormat { get; set; }

    [JsonProperty("tool_choice")]
    public string? ToolChoice { get; set; }
}

public class AssistantRunTool
{
    [JsonProperty("type")]
    public string? Type { get; set; }
}

public class AssistantRunTruncationStrategy
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("last_messages")]
    public int? LastMessages { get; set; }
}