namespace Pedrabotnik.Models;

public class AmoSource
{
    /// <summary>
    /// Необязательное поле. Идентификатор источника чата на стороне интеграции. Длина поля 40 символов, можно использовать любые печатные ascii символы и пробел. Если указывать источник не требуется, то поле source передавать не требуется.
    /// </summary>
    public string? external_id { get; set; }
}