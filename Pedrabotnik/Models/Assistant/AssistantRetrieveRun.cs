using Newtonsoft.Json;
using Pedrabotnik.Models.Assistant.Enums;

namespace Pedrabotnik.Models.Assistant;

public class AssistantRetrieveRun
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

    [JsonProperty("required_action")]
    public RequiredAction RequiredAction { get; set; }

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
    public object LastError { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("instructions")]
    public string Instructions { get; set; }

    [JsonProperty("tools")]
    public List<Tool> Tools { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, object> Metadata { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }

    [JsonProperty("temperature")]
    public double? Temperature { get; set; }

    [JsonProperty("top_p")]
    public double? TopP { get; set; }

    [JsonProperty("max_prompt_tokens")]
    public int? MaxPromptTokens { get; set; }

    [JsonProperty("max_completion_tokens")]
    public int? MaxCompletionTokens { get; set; }

    [JsonProperty("truncation_strategy")]
    public TruncationStrategy TruncationStrategy { get; set; }

    [JsonProperty("response_format")]
    public string ResponseFormat { get; set; }

    [JsonProperty("tool_choice")]
    public string ToolChoice { get; set; }

    [JsonProperty("parallel_tool_calls")]
    public bool ParallelToolCalls { get; set; }
}

public class RequiredAction
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("submit_tool_outputs")]
    public SubmitToolOutputs SubmitToolOutputs { get; set; }
}

public class SubmitToolOutputs
{
    [JsonProperty("tool_calls")]
    public List<ToolCall> ToolCalls { get; set; }
}

public class ToolCall
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public Function Function { get; set; }
}

public class Function
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("arguments")]
    public string Arguments { get; set; }
}

public class Tool
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("function")]
    public Function Function { get; set; }
}

public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}

public class TruncationStrategy
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("last_messages")]
    public object LastMessages { get; set; }
}