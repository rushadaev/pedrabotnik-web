namespace Pedrabotnik.Models;

public class AuthRequest
{
    public string account_id { get; set; }
    public string hook_api_version	 { get; set; }
    public string title { get; set; }
    public string scope_id { get; set; }
}