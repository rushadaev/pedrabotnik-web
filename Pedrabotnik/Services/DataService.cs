namespace Pedrabotnik.Services;

public class DataService
{
    public async Task<string> GetContractInfo(int contractNumber)
    {
        HttpClient _httpClient = new HttpClient();

        var res = await _httpClient.GetAsync(
            $"https://xn--80achdsmthemz.xn--p1ai/bot_order_info/?ORDER={contractNumber}&GUID=XBxvUhbqweLlsG");

        if (res.IsSuccessStatusCode)
        {
            var json = await res.Content.ReadAsStringAsync();

            return json;
        }
        else
        {
            return "Не удалось получить данные.";
        }
    }
}