using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using Pedrabotnik.Models;
using Pedrabotnik.Services;
using Telegram.Bot;

namespace Pedrabotnik.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AmoController : ControllerBase
{
    /*
     account[subdomain]: dpoippk
     account[id]: 31579862
     account[_links][self]: https://dpoippk.amocrm.ru
     message[add][0][id]: b26bbe6d-a691-4a8b-b269-29df5fb9dbea
     message[add][0][chat_id]: f7432b58-39ed-4ee3-a39a-9bae88989406
     message[add][0][talk_id]: 166
     message[add][0][contact_id]: 13311307
     message[add][0][text]: test!
     message[add][0][created_at]: 1711548814
     message[add][0][type]: incoming
     message[add][0][author][id]: aaac520b-1d81-49e1-bdd3-f43d34341544
     message[add][0][author][type]: external
     message[add][0][author][name]: Adrbill
     message[add][0][origin]: onlinechat
     */
    
    private readonly IAssistantService _assistantService;
    private readonly IAmoCRMService _amoCrmService;
    private IDistributedCache _cache;
    
    private readonly string _secret = "1291b98bc6bcdc6bcf33294325901ff7c6e7932d";
    
    
    public AmoController(IAssistantService assistantService,
        IDistributedCache cache, IAmoCRMService amoCrmService)
    {
        _assistantService = assistantService;
        _amoCrmService = amoCrmService;
        _cache = cache;
    }
    // chatid 84a07796-7bef-468d-ac08-e97991354d77

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        try
        {
            return Ok();
            // if (messageHookV2 is not null)
            // {
            //     await botClient.SendTextMessageAsync(
            //         chatId: 220610073,
            //         text: $"Поймал сообщение c V2 хука" +
            //               $"\n\n{JsonConvert.SerializeObject(messageHookV2)}");
            // }

            string botLogMessage = string.Empty;
            
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
                botLogMessage += $"\n{key}: {Request.Form[key]}";
            }

            
            
            // Создаем объект Account
            var account = new Account
            {
                Subdomain = Request.Form["account[subdomain]"].FirstOrDefault(),
                Id = Request.Form["account[id]"].FirstOrDefault(),
                _links = new Links
                {
                    Self = Request.Form["account[_links][self]"].FirstOrDefault()
                }
            };

            // Создаем объект Message
            var message = new Models.Message
            {
                Add = new List<MessageEvent>
                {
                    new MessageEvent
                    {
                        Id = Request.Form["message[add][0][id]"].FirstOrDefault(),
                        ChatId = Request.Form["message[add][0][chat_id]"].FirstOrDefault(),
                        // TalkId = int.Parse(Request.Form["message[add][0][talk_id]"].FirstOrDefault()),
                        // ContactId = int.Parse(Request.Form["message[add][0][contact_id]"].FirstOrDefault()),
                        Text = Request.Form["message[add][0][text]"].FirstOrDefault(),
                        // CreatedAt = long.Parse(Request.Form["message[add][0][created_at]"].FirstOrDefault()),
                        // ElementType = int.Parse(Request.Form["message[add][0][element_type]"].FirstOrDefault()),
                        EntityType = Request.Form["message[add][0][entity_type]"].FirstOrDefault(),
                        // ElementId = int.Parse(Request.Form["message[add][0][element_id]"].FirstOrDefault()),
                        // EntityId = int.Parse(Request.Form["message[add][0][entity_id]"].FirstOrDefault()),
                        Type = Request.Form["message[add][0][type]"].FirstOrDefault(),
                        Author = new Author
                        {
                            Id = Request.Form["message[add][0][author][id]"].FirstOrDefault(),
                            Type = Request.Form["message[add][0][author][type]"].FirstOrDefault(),
                            Name = Request.Form["message[add][0][author][name]"].FirstOrDefault()
                        },
                        Origin = Request.Form["message[add][0][origin]"].FirstOrDefault()
                    }
                }
            };

            // Создаем объект AmoCrmWebhook
            var webhook = new AmoCrmWebhook
            {
                Account = account,
                Message = message
            };

            // await botClient.SendTextMessageAsync(
            //     chatId: 220610073,
            //     text: JsonConvert.SerializeObject(webhook.Message.Add[0]));

            string messageText = webhook.Message.Add.First().Text;
            string chatId = webhook.Message.Add.First().ChatId;

            var auth = await _amoCrmService.Auth();

            AuthRequest? authRequest =
                JsonConvert.DeserializeObject<AuthRequest>(await auth.Content.ReadAsStringAsync());

            if (authRequest is null)
                return Ok();

            string scope_id = authRequest.scope_id;
            
            double milliseconds = (double)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            long roundedMilliseconds = (long)Math.Round(milliseconds, MidpointRounding.AwayFromZero);

            AmoSendMessageRequest body = new AmoSendMessageRequest()
            {
                event_type = "new_message",
                payload = new AmoPayload()
                {
                    timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                    msec_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    msgid = Guid.NewGuid().ToString(),
                    conversation_id = webhook.Message.Add.First().ChatId,
                    conversation_ref_id = webhook.Message.Add.First().ChatId,
                    // sender = new AmoSenderReceiver()
                    // {
                    //     id = "46f8af97-0286-4bfa-b93a-3dd93014040d",
                    //     ref_id = "46f8af97-0286-4bfa-b93a-3dd93014040d",
                    //     name = "PedrabotnikBot"
                    // },
                    receiver = new AmoSenderReceiver()
                    {
                        id = webhook.Message.Add.First().Author.Id,
                        name = webhook.Message.Add.First().Author.Name ?? ""
                    },
                    message = new AmoMessage()
                    {
                        type = "text",
                        text = "Ответное сообщение от бота!",
                    },
                    silent = false
                }
            };
            
            // await botClient.SendTextMessageAsync(
            //     chatId: 220610073,
            //     text: "Тело запроса:" +
            //           $"\n\n{JsonConvert.SerializeObject(body)}");
            
            var method = "POST";
            var contentType = "application/json";
            var date = DateTimeOffset.Now.ToString("r");
            var path = $"/v2/origin/custom/{scope_id}";
            var accountId = "75877236-8d41-4d51-bec3-2f63ff1de16e";
            var secret = "1291b98bc6bcdc6bcf33294325901ff7c6e7932d";
            var url = "https://amojo.amocrm.ru" + path;
            
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

            string responseContent = await response.Content.ReadAsStringAsync();

            // await botClient.SendTextMessageAsync(
            //     chatId: 220610073,
            //     text: "Результат выполнения запроса на отправку сообщения в ответ:" +
            //           $"\n\n{responseContent}");

            return Ok();
            
            // обработать сообщение
            string threadId;
                    
            // var chatThread = await _cache.GetStringAsync(chatId);
            string? chatThread = null;
            
            if (chatThread != null)
            {
                threadId = chatThread;
            }
            else
            {
                var thread = await _assistantService.CreateThread();
                threadId = thread.Id;
                
                await _cache.SetStringAsync(chatId, thread.Id, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365),
                });
            }
            
            await _assistantService.AddMessage(threadId, messageText);
            
            var run = await _assistantService.RunAssistant(threadId);
                             
            var runStatus = await _assistantService.CheckStatus(threadId, run.Id);
                             
            while (runStatus.Status != RunStatus.Completed && runStatus.Status != RunStatus.Failed)
            {
                await Task.Delay(1500);
                runStatus = await _assistantService.CheckStatus(threadId, run.Id);
            }
            
            var assistantResponse = await _assistantService.GetResponse(threadId);
            
            return Ok();
        }
        catch (Exception e)
        {
            TelegramBotClient botClient = new TelegramBotClient("6688879288:AAGmQRBqqUjUNFeM88R30s2KAfLovVtkVek");
            
            await botClient.SendTextMessageAsync(
                chatId: 220610073,
                text: $"Ex message: {e.Message}" +
                      $"\nEx JSON: \n{JsonConvert.SerializeObject(e)}");
            return BadRequest(e);
        }
    }

    
    private async Task<IActionResult> CreateAssistantFunction()
    {
        OpenAIClient _api = new OpenAIClient("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");

        // Function _function = new Function("get_current_weather", "Возвращает текущую погоду для указанного города", new JsonObject
        // {
        //     { "city", new JsonObject { { "type", "string" }, { "description", "Город" } } },
        // });
        
        var agentService = new InviteAgentService();
        var dataService = new DataService();
        var assistantRequest = new CreateAssistantRequest(
            tools: new List<Tool> { Tool.GetOrCreateTool(dataService, nameof(dataService.GetContractInfo), "Возвращает информацию о контракте."), Tool.GetOrCreateTool(agentService, nameof(agentService.InviteAgent)) },
            files: null,
            instructions: "Ты можешь использовать функцию GetContractInfo, чтобы узнать информацию о заказе, и InviteAgent, чтобы сообщить о необходимости подключения оператора.");
        var assistant = await _api.AssistantsEndpoint.RetrieveAssistantAsync("asst_wFY47nOYoXqSAvIemuqqNEwK");
        var res = await assistant.ModifyAsync(assistantRequest);
        return Ok();

    }
}