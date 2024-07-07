namespace Pedrabotnik.Models;

public class AmoCrmWebhook
{
    public Account? Account { get; set; }
    public Message? Message { get; set; }
}


public class Account
{
    public string? Subdomain { get; set; }
    public string? Id { get; set; }
    public Links? _links { get; set; }
}

public class Links
{
    public string? Self { get; set; }
}

public class BaseMessage
{
    public List<ChatMessage>? Add { get; set; }
    public List<ChatMessage>? Update { get; set; }
}

public class ChatMessage
{
    public string? Id { get; set; }
    public string? ChatId { get; set; }
    public string? TalkId { get; set; }
    public string? ContactId { get; set; }
    public string? Text { get; set; }
    public string? CreatedAt { get; set; }
    public string? Type { get; set; }
    public Author? Author { get; set; }
    public string? Origin { get; set; }
    public Contact? Contact { get; set; }
    public int? ResponsibleUserId { get; set; }
    public string? Status { get; set; }
    public int? MessagesCount { get; set; }
    public int? UnreadMessagesCount { get; set; }
    public ChatMessage? LastMessage { get; set; }
}

public class Author
{
    public string? Id { get; set; }
    public string? Type { get; set; }

    public string? Name { get; set; }
}

public class Contact
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class Message
{
    public List<MessageEvent>? Add { get; set; }
    public List<MessageEvent>? Update { get; set; }
}

public class MessageEvent
{
    public string? Id { get; set; }
    public string? ChatId { get; set; }
    public int? TalkId { get; set; }
    public int? ContactId { get; set; }
    public string? Text { get; set; }
    public long? CreatedAt { get; set; }
    
    public int? ElementType { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }

    public int? ElementId { get; set; }
    public string? Type { get; set; }
    public Author? Author { get; set; }
    public string? Origin { get; set; }
}



