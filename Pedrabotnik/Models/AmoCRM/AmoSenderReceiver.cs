namespace Pedrabotnik.Models;

public class AmoSenderReceiver
{
    /// <summary>
    /// Идентификатор участника чата на стороне интеграции
    /// </summary>
    public string id { get; set; }
    
    /// <summary>
    /// Идентификатор участника чата на стороне API Чатов
    /// </summary>
    public string? ref_id { get; set; }
    
    /// <summary>
    /// Имя участника чата
    /// </summary>
    public string name { get; set; }
    
    /// <summary>
    /// Ссылка на аватар участника чата. Ссылка должен быть доступна для сторонних ресурсов и отдавать изображение для скачивания
    /// </summary>
    public string? avatar { get; set; }
    
    /// <summary>
    /// Профиль участника чата.
    /// </summary>
    public AmoSenderReceiverProfile? profile { get; set; }
    
    /// <summary>
    /// Ссылка на профиль участника чата в сторонней чат системе
    /// </summary>
    public string? profile_link { get; set; }
}