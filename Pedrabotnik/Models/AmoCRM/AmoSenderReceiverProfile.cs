namespace Pedrabotnik.Models;

public class AmoSenderReceiverProfile
{
    /// <summary>
    /// Телефон. При создании нового неразобранного будет добавлен в данные контакта
    /// </summary>
    public string? phone { get; set; }
    
    /// <summary>
    /// Email. При создании нового неразобранного будет добавлен в данные контакта
    /// </summary>
    public string? email { get; set; }
}