namespace Pedrabotnik.Models;

public class AmoSendMessageRequest
{
    public string event_type { get; set; } = "new_message";
    public AmoPayload payload { get; set; }
}