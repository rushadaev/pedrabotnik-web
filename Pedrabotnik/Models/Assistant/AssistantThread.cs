using Newtonsoft.Json;

namespace Pedrabotnik.Models.Assistant;

public class AssistantThread
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, string> Metadata { get; set; }

    [JsonProperty("tool_resources")]
    public Dictionary<string, string> ToolResources { get; set; }
}