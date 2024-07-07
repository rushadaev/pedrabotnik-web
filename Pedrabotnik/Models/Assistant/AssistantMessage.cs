using Newtonsoft.Json;

namespace Pedrabotnik.Models.Assistant;

public class AssistantMessage
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("assistant_id")]
    public object AssistantId { get; set; }

    [JsonProperty("thread_id")]
    public string ThreadId { get; set; }

    [JsonProperty("run_id")]
    public object RunId { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public List<Content> Content { get; set; }

    [JsonProperty("attachments")]
    public List<object> Attachments { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, object> Metadata { get; set; }
}

public class Content
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("text")]
    public Text Text { get; set; }
}

public class Text
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("annotations")]
    public List<object> Annotations { get; set; }
}