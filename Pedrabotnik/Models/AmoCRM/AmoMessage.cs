namespace Pedrabotnik.Models;

public class AmoMessage
{
    public string type { get; set; }
    public string text { get; set; }
    public string media { get; set; } = string.Empty;
    public string file_name { get; set; } = string.Empty;
    public int? file_size { get; set; } = null;
    public string? sticker_id { get; set; }
    public AmoMessageLocation? location { get; set; }
    public AmoMessageContact? contact { get; set; }
    public string? callback_data { get; set; }
}