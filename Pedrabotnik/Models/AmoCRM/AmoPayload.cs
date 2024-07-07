namespace Pedrabotnik.Models;

public class AmoPayload
{
    /// <summary>
    /// Время сообщения, метка unix
    /// </summary>
    public int timestamp { get; set; }
    
    /// <summary>
    /// Время сообщения в миллисекундах
    /// </summary>
    public long msec_timestamp { get; set; }
    
    /// <summary>
    /// Тип события, в данный момент поддерживается только new_message и edit_message
    /// </summary>
    // public string event_type { get; set; } = "new_message";
    
    /// <summary>
    /// Идентификатор чата на стороне интеграции
    /// </summary>
    public string conversation_id { get; set; }

    /// <summary>
    /// Идентификатор чата на стороне amoCRM, необязательное поле. Необходимо передавать, когда клиент ответит на сообщение отправленное с помощью "Написать первым", чтобы API чатов связало чат на вашей стороне с чатом в системе.
    /// </summary>
    public string? conversation_ref_id { get; set; }

    /// <summary>
    /// Нужно ли создавать неразобранное и отправлять уведомление по сообщению в аккаунте amoCRM. При редактировании сообщения неразобранное не создаётся и уведомление не отправляется.
    /// </summary>
    public bool silent { get; set; } = true;

    /// <summary>
    /// Необязательное поле. Если указывать конкретный источник не требуется, то поле source передавать не требуется. При редактировании сообщения поле будет пригнорировано.
    /// </summary>
    public AmoSource? source { get; set; }

    /// <summary>
    /// Отправитель сообщения. При редактировании сообщения поле будет пригнорировано.
    /// </summary>
    public AmoSenderReceiver sender { get; set; }
    
    /// <summary>
    /// Получатель сообщения. При редактировании сообщения поле будет пригнорировано.
    /// </summary>
    public AmoSenderReceiver receiver { get; set; }
    
    /// <summary>
    /// Идентификатор сообщения чата на стороне amoCRM, необязательное поле. Может передаваться только при редактирование сообщение.
    /// </summary>
    public string? id { get; set; }
    
    /// <summary>
    /// Идентификатор сообщения чата на стороне интеграции. Если при редактировании сообщения передан вместе с id, то msgid будет установлен в качестве идентификатора сообщения на стороне интеграции.
    /// </summary>
    public string msgid { get; set; }
    
    /// <summary>
    /// Обязательное поле. Объект входящего сообщения.
    /// </summary>
    public AmoMessage message { get; set; }
    
    /// <summary>
    /// Необязательное поле. Объект цитаты c ответом. При редактировании сообщения поле будет пригнорировано.
    /// </summary>
    public AmoReplyTo? reply_to { get; set; }
    
    /// <summary>
    /// Необязательное поле. Объект цитаты с перессылкой. При редактировании сообщения поле будет пригнорировано.
    /// </summary>
    public AmoForwards? forwards { get; set; }
    
    /// <summary>
    /// Необязательное поле. Может передаватся только при редактировании сообщения. Объект статуса доставки сообщения.
    /// </summary>
    public AmoDeliveryStatus? delivery_status { get; set; }
    
}