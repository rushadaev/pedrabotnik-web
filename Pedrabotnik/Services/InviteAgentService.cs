using Pedrabotnik.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Pedrabotnik.Services;
public class InviteAgentService
{
    public async Task<string> InviteAgent(string client_id, string chat_id)
    {
        HttpClient client = new HttpClient();
        
        string responseUrl =
            "https://bot.jivosite.com/webhooks/CWwS2BMuup5BQ0f/7060fd6d-341a-49ac-b933-29b7b681e7c6";

        var response = await client.PostAsJsonAsync(responseUrl, new
        {
            id = Guid.NewGuid().ToString(),
            client_id = client_id,
            chat_id = chat_id,
            @event = "INVITE_AGENT"
        });

        string resposneRes = response.IsSuccessStatusCode ? "Succes" : "Failed";

        ITelegramBotClient _botClient = new TelegramBotClient("6688879288:AAGmQRBqqUjUNFeM88R30s2KAfLovVtkVek");

        await _botClient.SendTextMessageAsync(
            chatId: 220610073,
            text: $"```\nclient_id: {client_id}\nchat_id: {chat_id}\nresponse result: {resposneRes}\n```",
            parseMode: ParseMode.Markdown);

        if (response.IsSuccessStatusCode)
        {
            JivoController.disabledChats.Add(chat_id);
            return "";
        }

        return string.Empty;
    }
}