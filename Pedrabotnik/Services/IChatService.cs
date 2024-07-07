using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Pedrabotnik.Models.Assistant;

namespace Pedrabotnik.Services;

public interface IChatService
{
    public Task<AssistantThread?> CreateThread();
    
    public Task<AssistantMessage?> AddMessage(string threadId, string message);
    
    public Task<AssistantRun?> RunAssistant(string threadId);
    
    public Task<AssistantRetrieveRun?> CheckStatus(string threadId, string runId);
    
    public Task CallFunction(string threadId, string runId, string? client_id, string? chat_id);
    
    public Task<string> GetResponse(string threadId);
    //
    // public Task<string> SendMessageGPT(string message);
}

public class ChatService : IChatService
{
    private readonly HttpClient _client;
    private readonly string _key = "sk-proj-wR4HcoW6h96K55dmV4Y9T3BlbkFJRAyXu9disHqOi4xjzeyn";
    private readonly string _assistantId = "asst_pJOjTyvvUkqrex2ZiDke6bUU";

    public ChatService()
    {
        _client =  new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _key);
        _client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
    }
    
    public async Task<AssistantThread?> CreateThread()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/threads");
        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var threadResponse = JsonConvert.DeserializeObject<AssistantThread>(responseBody);
            
            if (threadResponse is not null)
                return threadResponse;
        }

        return null;
    }

    public async Task<AssistantMessage?> AddMessage(string threadId, string message)
    {
        var requestJson = new
        {
            role = "user",
            content = message
        };

        string json = JsonConvert.SerializeObject(requestJson);
        HttpResponseMessage response = await _client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", new StringContent(json, Encoding.UTF8, "application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var messageResponse = JsonConvert.DeserializeObject<AssistantMessage>(responseBody);
            
            if (messageResponse is not null)
                return messageResponse;
        }

        return null;
    }

    public async Task<AssistantRun?> RunAssistant(string threadId)
    {
        var requestJson = new
        {
            assistant_id = _assistantId,
        };

        string json = JsonConvert.SerializeObject(requestJson);
        HttpResponseMessage response = await _client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", new StringContent(json, Encoding.UTF8, "application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var runResponse = JsonConvert.DeserializeObject<AssistantRun>(responseBody);
            
            if (runResponse is not null)
                return runResponse;
        }

        return null;
    }

    public async Task<AssistantRetrieveRun?> CheckStatus(string threadId, string runId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var runResponse = JsonConvert.DeserializeObject<AssistantRetrieveRun>(responseBody);
            
            if (runResponse is not null)
                return runResponse;
        }

        return null;
    }

    public async Task CallFunction(string threadId, string runId, string? client_id, string? chat_id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var runResponse = JsonConvert.DeserializeObject<AssistantRetrieveRun>(responseBody);
        string? functionName = runResponse?.RequiredAction.SubmitToolOutputs.ToolCalls[0].Function.Name;

        if (functionName is "getContractInfo")
        {
            DataService service  = new DataService();
            
            string argumentsJson = runResponse?.RequiredAction.SubmitToolOutputs.ToolCalls[0].Function.Arguments;
            
            dynamic arguments = JsonConvert.DeserializeObject(argumentsJson);
            
            int contractNumber = arguments.contractNumber;
            
            string resp = await service.GetContractInfo(contractNumber);
            
            var jsonData = new
            {
                tool_outputs = new[]
                {
                    new
                    {
                        tool_call_id = runResponse?.RequiredAction.SubmitToolOutputs.ToolCalls[0].Id,
                        output = resp
                    }
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);
            
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            await _client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}/submit_tool_outputs", content);
        }
        else if (functionName is "initRate")
        {
            await _client.PostAsJsonAsync(
                $"https://bot.jivosite.com/webhooks/CWwS2BMuup5BQ0f/7060fd6d-341a-49ac-b933-29b7b681e7c6", new
                {
                    @event = "INIT_RATE",
                    id = Guid.NewGuid().ToString(),
                    client_id = client_id,
                    chat_id = chat_id
                });
            
            string responseContent = await response.Content.ReadAsStringAsync();

            var jsonData = new
            {
                tool_outputs = new[]
                {
                    new
                    {
                        tool_call_id = runResponse?.RequiredAction.SubmitToolOutputs.ToolCalls[0].Id,
                        output = "Function is called."
                    }
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);
            
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            await _client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}/submit_tool_outputs", content);
        }
        else
        {
            InviteAgentService agentService = new InviteAgentService();

            string res = await agentService.InviteAgent(client_id, chat_id);
            
            var jsonData = new
            {
                tool_outputs = new[]
                {
                    new
                    {
                        tool_call_id = runResponse?.RequiredAction.SubmitToolOutputs.ToolCalls[0].Id,
                        output = "Менеджер вызван."
                    }
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);
            
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            await _client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}/submit_tool_outputs", content);
        }
    }
    
    public async Task<string> GetResponse(string threadId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.openai.com/v1/threads/{threadId}/messages");
        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var runResponse = JsonConvert.DeserializeObject<AssistantMessageList>(responseBody);
            
            if (runResponse is not null)
                return CleanText(runResponse.Messages.FirstOrDefault().Content.FirstOrDefault().Text.Value);
        }

        return null;
    }

    public async Task<string> SendMessageGPT(string message)
    {
        return string.Empty;
        // using var api = new OpenAIClient(_key);
        //
        // var messages = new List<Message>
        // {
        //     new Message(Role.System, "### Role:\nYour role is a Russian expert in communication analysis, Russian psychology, customer service and manager. Your expertise covers dialog systems, machine learning, perfect natural language processing. You fully understand dialog context and hidden meanings. You are a specialist in the company \"Durov House\". Your company sells modular houses. \n\n#### Purpose:\nYour goal is to provide a perfect and truthful report of the provided dialog between manager and client in Russian. And to extract the information and compile this information into a concise clear and as accurate report as possible in no more than 200 words. \n\n ### Explicit Report Template:\n- Purpose of contact/need: [Description of what the customer is looking for or wants to know]\n- Interest: [Which options the customer is interested in]\n- Customer Objections: [Which Full description of the client's objections]\n- Sales manager suggestions: [Explicit Description of suggestions made to the customer]\n- Professional recommendations (from you for the next step) : [Suggestion for further interaction with the client. Tips on how to better sell to this customer based on the dialog]\n\n #### Limitations: \n1. The number of words in the report is 100 to 150 words.\n2. It is forbidden to step out of role. \n3. It is forbidden to make up facts unrelated to the dialog.\n4. It is forbidden to give false information.\n5. You provide reports in perfect ONLY Russian.\n6. Before you begin, take a deep breath. \n7. The report should not contain unnecessary information and should be clearly structured according to a template.  \n"),
        //     new Message(Role.User, message),
        // };
        //
        // var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo_16K);
        // var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        // var choice = response.FirstChoice;
        //
        // Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");
        //
        // return choice.Message;
    }
    
    public static string CleanText(string input)
    {
        // Убираем все  -подобные символы
        string pattern1 = @"【\d+:\d+†source】";
        string result = Regex.Replace(input, pattern1, string.Empty);

        // Оставляем только ссылки в круглых скобках
        string pattern2 = @"\[[^\]]+\]\((.*?)\)";
        result = Regex.Replace(result, pattern2, "$1");

        // Убираем все **
        string pattern3 = @"\*\*";
        result = Regex.Replace(result, pattern3, "*");

        return result;
    }
}