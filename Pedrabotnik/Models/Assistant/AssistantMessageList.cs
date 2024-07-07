using Newtonsoft.Json;

namespace Pedrabotnik.Models.Assistant;

public class AssistantMessageList
{
    [JsonProperty("object")]
    public string? ObjectType { get; set; }

    [JsonProperty("data")]
    public List<Message>? Messages { get; set; }

    [JsonProperty("first_id")]
    public string? FirstId { get; set; }

    [JsonProperty("last_id")]
    public string? LastId { get; set; }

    [JsonProperty("has_more")]
    public bool? HasMore { get; set; }
}

public class Message
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("object")]
    public string? ObjectType { get; set; }

    [JsonProperty("created_at")]
    public long? CreatedAt { get; set; }

    [JsonProperty("assistant_id")]
    public object? AssistantId { get; set; }

    [JsonProperty("thread_id")]
    public string? ThreadId { get; set; }

    [JsonProperty("run_id")]
    public object? RunId { get; set; }

    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("content")]
    public List<ContentItem> Content { get; set; }

    [JsonProperty("attachments")]
    public List<object> Attachments { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, object> Metadata { get; set; }
}

public class ContentItem
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("text")]
    public TextItem Text { get; set; }
}

public class TextItem
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("annotations")]
    public List<object> Annotations { get; set; }
}
