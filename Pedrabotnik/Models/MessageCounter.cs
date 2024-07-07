namespace Pedrabotnik.Models;

public class MessageCounter
{
    public int Count { get; set; } = 0;
    public string ChatId { get; set; }
    
    public void Increment() => Count++;
}