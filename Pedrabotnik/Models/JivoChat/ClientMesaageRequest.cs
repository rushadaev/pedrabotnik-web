namespace Pedrabotnik.Models.JivoChat;

public class ClientMesaageRequest
{
    public string? id { get; set; }
    public string? site_id { get; set; }
    public string? client_id { get; set; }
    public string? chat_id { get; set; }
    public bool? agents_online { get; set; }
    public ClientMesaageRequestSender? sender { get; set; }

    public Rate? rate { get; set; }
    public ClientMesaageRequestMessage? message { get; set; }
    public ClientMesaageRequestChannel? channel { get; set; }
    public string? @event { get; set; }
    
}

public class ClientMesaageRequestChannel
{
    public string? id { get; set; }
    public string? type { get; set; }
}

public class ClientMesaageRequestSender
{
    public int? id { get; set; }
    public string? url { get; set; }
    public bool? has_contacts { get; set; }
}

public class ClientMesaageRequestMessage
{
    public string? type { get; set; }
    public string? text { get; set; }
    public int? timestamp { get; set; }
}

public class Rate
{
    public string rating { get; set; }
    public string comment { get; set; }
    public long timestamp { get; set; }
}