namespace Pedrabotnik.Models;

public class AmoForwards
{
    /// <summary>
    /// Массив объектов вложенных сообщений, на данный момент нельзя переслать более 1 сообщения. Сообщения из цитаты с перессылкой могут принадлежать любому внешнему чату, что принадлежит интеграции.
    /// </summary>
    public List<AmoMessage> messages { get; set; }
    
    /// <summary>
    /// Идентификатор чата на стороне API чатов. Чат обязательно должен принадлежать интеграции
    /// </summary>
    public string? conversation_ref_id	 { get; set; }
    
    /// <summary>
    /// Идентификатор чата на стороне интеграции.
    /// </summary>
    public string? conversation_id { get; set; }
}