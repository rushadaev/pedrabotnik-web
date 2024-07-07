using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Pedrabotnik.Services;

public interface IAmoCRMService
{
    Task<HttpResponseMessage> Auth();
}

public class AmoCRMService : IAmoCRMService
{
    private readonly string _secret = "1291b98bc6bcdc6bcf33294325901ff7c6e7932d";

    public async Task<HttpResponseMessage> Auth()
    {
        var method = "POST";
        var contentType = "application/json";
        var date = DateTimeOffset.Now.ToString("r");
        var path = "/v2/origin/custom/cae54975-5321-4e70-90f8-4ce30c890151/connect";
        var accountId = "75877236-8d41-4d51-bec3-2f63ff1de16e";
        var secret = "1291b98bc6bcdc6bcf33294325901ff7c6e7932d";
        var url = "https://amojo.amocrm.ru" + path;
        var body = new
        {
            account_id = accountId,
            title = "PedrabotnikBot",
            hook_api_version = "v2",
        };

        var requestBody = JsonConvert.SerializeObject(body);
        var contentMd5 = MD5.HashData(Encoding.UTF8.GetBytes(requestBody));
        var contentMd5Base64 = BitConverter.ToString(contentMd5).Replace("-", "").ToLower();

        var str = string.Join("\n", method, contentMd5Base64, contentType, date, path);

        var secretBytes = Encoding.UTF8.GetBytes(secret);

        using var hmac = new HMACSHA1(secretBytes);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
        var signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        var client = new HttpClient();
        var content = new StringContent(requestBody, null, contentType);
        
        // md5
        content.Headers.ContentMD5 = Convert.FromBase64String(contentMd5Base64);
        // date
        client.DefaultRequestHeaders.Date = DateTimeOffset.Parse(date);
        // x-signature
        content.Headers.TryAddWithoutValidation("X-Signature", signature);
        
        // content-type
        if (content.Headers.ContentType != null)
        {
            content.Headers.ContentType.MediaType = contentType;
            content.Headers.ContentType.CharSet = "";
        }
        
        var response = await client.PostAsync(url, content);

        client.Dispose();
        
        return response;
    }
}
