using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class AmoCRMChatClient
{
    private readonly HttpClient _client;
    private readonly string _secret;

    public AmoCRMChatClient(string baseAddress, string secret)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        _secret = secret;
    }

    public async Task<HttpResponseMessage> SendPostRequestAsync(string path, object content)
    {
        var method = "POST";
        var contentType = "application/json";
        var date = DateTime.UtcNow.ToString("r");

        var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(content);
        var contentMd5 = GetMd5Hash(requestBody);

        var signatureString = $"{method}\n{contentMd5}\n{contentType}\n{date}\n{path}";
        var signature = GetHmacSha1Hash(signatureString, _secret);
        
        _client.DefaultRequestHeaders.Add("Date", date);
        _client.DefaultRequestHeaders.Add("Content-Type", contentType);
        // _client.DefaultRequestHeaders.Add("Content-MD5", contentMd5);
        _client.DefaultRequestHeaders.Add("X-Signature",  signature);

        var contentJson = new StringContent(requestBody, Encoding.UTF8, contentType);
        
        return await _client.PostAsync(path, contentJson);
    }

    private string GetMd5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }

    private string GetHmacSha1Hash(string data, string key)
    {
        using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
        {
            byte[] hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                sb.Append(hashValue[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
