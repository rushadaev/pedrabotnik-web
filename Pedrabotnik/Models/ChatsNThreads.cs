using System.ComponentModel.DataAnnotations;

namespace Pedrabotnik.Models;

public class ChatsNThreads
{
    [Key]
    public int id { get; set; }
    public string chat_id { get; set; }
    public string thread_id { get; set; }

    public ChatsNThreads(string chat_id, string thread_id)
    {
        this.chat_id = chat_id;
        this.thread_id = thread_id;
    }
}