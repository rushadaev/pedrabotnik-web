using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OpenAI.Threads;
using Pedrabotnik.Data;
using Pedrabotnik.Models;
using Pedrabotnik.Models.Assistant.Enums;
using Pedrabotnik.Models.JivoChat;
using Pedrabotnik.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Pedrabotnik.Controllers;

[ApiController]
public class JivoController : ControllerBase
{
    private static string baseUrl = "https://bot.jivosite.com";
    private static string subUrl = "/webhooks/";
    private static string provider_id = "CWwS2BMuup5BQ0f";
    private static string botToken = "7060fd6d-341a-49ac-b933-29b7b681e7c6";

    // private static string responseUrl = baseUrl + subUrl + provider_id + "/" + botToken;
    private static string responseUrl =
        "https://bot.jivosite.com/webhooks/CWwS2BMuup5BQ0f/7060fd6d-341a-49ac-b933-29b7b681e7c6";
    private ITelegramBotClient _botClient = new TelegramBotClient("6688879288:AAGmQRBqqUjUNFeM88R30s2KAfLovVtkVek");

    private long tgChatId = 220610073;
    
    private readonly IChatService  _chatService;
    
    private IDistributedCache _cache;

    public static List<string> disabledChats = new(); 
    
    public static List<MessageCounter> messageCounter = new();

    public JivoController(IDistributedCache cache, IChatService chatService)
    {
        _cache = cache;
        _chatService  = chatService;
    }
    
    [HttpPost("7060fd6d-341a-49ac-b933-29b7b681e7c6")]
    public async Task<IActionResult> HttpPost([FromBody] ClientMesaageRequest request)
    {
        if (request.message is not null)
        {
            var tasks = new[]
            {
                Task.Run(() => ResponseClient(request))
            };
        }
        
        return Ok(new { status = "received" });
    }

    private async Task ResponseClient(ClientMesaageRequest request)
    {
        try
        {
            if (disabledChats.Contains(request.chat_id))
            {
                return;
            }

            if (!messageCounter.Select(x => x.ChatId).Contains(request.chat_id))
            {
                messageCounter.Add(new MessageCounter()
                {
                    ChatId = request.chat_id,
                    Count = 1
                });
            }
            else
            {
                var curMessageCounter = messageCounter.FirstOrDefault(x  => x.ChatId == request.chat_id);

                if (curMessageCounter is not null)
                {
                    curMessageCounter.Increment();

                    if (curMessageCounter.Count == 4)
                    {
                        HttpClient client = new HttpClient();
                        await client.PostAsJsonAsync(
                            $"https://bot.jivosite.com/webhooks/CWwS2BMuup5BQ0f/7060fd6d-341a-49ac-b933-29b7b681e7c6", new
                            {
                                @event = "INIT_RATE",
                                id = Guid.NewGuid().ToString(),
                                client_id = request.client_id,
                                chat_id = request.chat_id
                            });
                        client.Dispose();
                    }
                }
            }

            await _botClient.SendTextMessageAsync(
                chatId: tgChatId,
                text: "```\n" + JsonConvert.SerializeObject(request) + "\n```",
                parseMode: ParseMode.Markdown);

            if (request.rate is not null)
            {
                if (request.rate.rating == "bad" | request.rate.rating == "badnormal")
                {
                    InviteAgentService agentService = new InviteAgentService();

                    string res = await agentService.InviteAgent(request.client_id, request.chat_id);
                }
            }
            
            string threadId;
                        
            var chatThread = await _cache.GetStringAsync(request.chat_id);
                
            if (chatThread != null)
            {
                threadId = chatThread;
            }
            else
            {
                DatabaseContext _context = new DatabaseContext();
                
                ChatsNThreads? currChat = await _context.ChatsNThreads.FirstOrDefaultAsync(x => x.chat_id == request.chat_id);

                if (currChat is null)
                {
                    var thread = await _chatService.CreateThread();
                    
                    threadId = thread.Id;

                    await _context.ChatsNThreads.AddAsync(new ChatsNThreads(request.chat_id, thread.Id));
                    
                    await _context.SaveChangesAsync();
                    
                    await _cache.SetStringAsync(request.chat_id, thread.Id, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    });
                }
                else
                {
                    threadId = currChat.chat_id;
                }
            }
                
            await _chatService.AddMessage(threadId, request.message.text);
                
            var run = await _chatService.RunAssistant(threadId);
            
            while (run is null)
            {
                await Task.Delay(1000);
                run = await _chatService.RunAssistant(threadId);
            }
            
            var runStatus = await _chatService.CheckStatus(threadId, run.Id);
                                 
            while (runStatus.Status != AssistantRunStatus.Completed && runStatus.Status != AssistantRunStatus.Failed)
            {
                if (runStatus.Status == AssistantRunStatus.RequiresAction)
                {
                    await _chatService.CallFunction(threadId, run.Id, request.client_id, request.chat_id);
                }
                await Task.Delay(1500);
                runStatus = await _chatService.CheckStatus(threadId, run.Id);
            }
                
            var assistantResponse = await _chatService.GetResponse(threadId);
            
            await _botClient.SendTextMessageAsync(
                chatId: tgChatId,
                text: $"Ответ assistant: {assistantResponse}");

            var responseBody = new
            {
                id = Guid.NewGuid().ToString(),
                @event = "BOT_MESSAGE",
                message = new
                {
                    type = "TEXT",
                    text = assistantResponse,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                },
                client_id = request.client_id,
                chat_id = request.chat_id,
            };

            HttpClient _client = new HttpClient();

            var response = await _client.PostAsJsonAsync(responseUrl, responseBody);

            if (response.IsSuccessStatusCode)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: tgChatId,
                    text: "Ответ успешно отправлен.");
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    chatId: tgChatId,
                    text: $"Ответ не отправлен. Причины:" +
                          $"\n\n{await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception e)
        {
            await _botClient.SendTextMessageAsync(
                chatId: tgChatId,
                text: JsonConvert.SerializeObject(e));
        }
    }
}