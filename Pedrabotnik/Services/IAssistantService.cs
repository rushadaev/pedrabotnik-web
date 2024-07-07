using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Models;
using OpenAI.Threads;
using Telegram.Bot;
using Message = OpenAI.Chat.Message;

namespace Pedrabotnik.Services;

public interface IAssistantService
{
    public Task<ThreadResponse> CreateThread();

    public Task AddMessage(string threadId, string message);

    public Task<RunResponse> RunAssistant(string threadId);

    public Task<RunResponse> CheckStatus(string threadId, string runId);

    public Task CallFunction(string threadId, string runId, string client_id, string chat_id);

    public Task<string> GetResponse(string threadId);

    public Task<string> SendMessageGPT(string message);
}

public class AssistantService : IAssistantService
{

    private readonly string _key = "sk-proj-wR4HcoW6h96K55dmV4Y9T3BlbkFJRAyXu9disHqOi4xjzeyn";
    // private readonly string _assistantId = "asst_Nnrak56PzcCQ2v3MDuYq4krS"; // Jivo-Bot-v4
    private readonly string _assistantId = "asst_McwQhkTgZYhK29gMqknO5qsF"; // Jivo-Bot-v5
    
    public async Task<ThreadResponse> CreateThread()
    {
        using var api = new OpenAIClient(_key);

        ThreadResponse thread = await api.ThreadsEndpoint.CreateThreadAsync();

        return thread;
    }

    public async Task AddMessage(string threadId, string message)
    {
        using var api = new OpenAIClient(_key);

        await api.ThreadsEndpoint.CreateMessageAsync(threadId, new CreateMessageRequest(message));
    }

    public async Task<RunResponse> RunAssistant(string threadId)
    {
        using var api = new OpenAIClient(_key);
        
        var run = await api.ThreadsEndpoint.CreateRunAsync(threadId, new CreateRunRequest(_assistantId, temperature: 0.69));
        return run;
    }

    public async Task<RunResponse> CheckStatus(string threadId, string runId)
    {
        using var api = new OpenAIClient(_key);

        var status = await api.ThreadsEndpoint.RetrieveRunAsync(threadId, runId);

        status = await status.UpdateAsync();

        return status;
    }

    public async Task CallFunction(string threadId, string runId, string client_id, string chat_id)
    {
        OpenAIClient api = new OpenAIClient(_key);
        
        var ass = await api.AssistantsEndpoint.RetrieveAssistantAsync(_assistantId);
        
        var status = await api.ThreadsEndpoint.RetrieveRunAsync(threadId, runId);

        status = await status.UpdateAsync();

        if (status.RequiredAction.SubmitToolOutputs.ToolCalls.FirstOrDefault().FunctionCall.Name.Contains("InviteAgent"))
        {
            InviteAgentService agentService = new InviteAgentService();

            string res = await agentService.InviteAgent(client_id, chat_id);

            var outputs = status.RequiredAction.SubmitToolOutputs.ToolCalls.AsEnumerable();
            
            await status.SubmitToolOutputsAsync(new SubmitToolOutputsRequest(new List<ToolOutput>() { new ToolOutput(outputs.First().Id, "Агент вызван.")}));
        }
        else
        {
            DataService dataService = new DataService();
                        
            var order = JsonConvert.DeserializeAnonymousType(status.RequiredAction.SubmitToolOutputs.ToolCalls[0]
                .FunctionCall.Arguments, new { contractNumber = 0 });
                        
            int contractNumber = order.contractNumber;
                        
            string orderInfo = await dataService.GetContractInfo(contractNumber);
            
            var outputs = status.RequiredAction.SubmitToolOutputs.ToolCalls.AsEnumerable();
            
            await status.SubmitToolOutputsAsync(new SubmitToolOutputsRequest(new List<ToolOutput>() { new ToolOutput(outputs.First().Id, orderInfo)}));
        }
    }
    
    public async Task<string> GetResponse(string threadId)
    {
        using var api = new OpenAIClient(_key);

        var res = await api.ThreadsEndpoint.ListMessagesAsync(threadId);
        
        // ITelegramBotClient botClient = new TelegramBotClient("6688879288:AAGmQRBqqUjUNFeM88R30s2KAfLovVtkVek");
        string pattern = @"【[^】]*】";
        string replacedText = Regex.Replace(res.Items[0].Content[0].Text.Value, pattern, "");
        // var response = res.Items[0].Content[0].Text.Value.Replace("【en4†источник】", "");
        
        return replacedText;
    }

    public async Task<string> SendMessageGPT(string message)
    {
        using var api = new OpenAIClient(_key);
        
        var messages = new List<Message>
        {
            new Message(Role.System, "### Role:\nYour role is a Russian expert in communication analysis, Russian psychology, customer service and manager. Your expertise covers dialog systems, machine learning, perfect natural language processing. You fully understand dialog context and hidden meanings. You are a specialist in the company \"Durov House\". Your company sells modular houses. \n\n#### Purpose:\nYour goal is to provide a perfect and truthful report of the provided dialog between manager and client in Russian. And to extract the information and compile this information into a concise clear and as accurate report as possible in no more than 200 words. \n\n ### Explicit Report Template:\n- Purpose of contact/need: [Description of what the customer is looking for or wants to know]\n- Interest: [Which options the customer is interested in]\n- Customer Objections: [Which Full description of the client's objections]\n- Sales manager suggestions: [Explicit Description of suggestions made to the customer]\n- Professional recommendations (from you for the next step) : [Suggestion for further interaction with the client. Tips on how to better sell to this customer based on the dialog]\n\n #### Limitations: \n1. The number of words in the report is 100 to 150 words.\n2. It is forbidden to step out of role. \n3. It is forbidden to make up facts unrelated to the dialog.\n4. It is forbidden to give false information.\n5. You provide reports in perfect ONLY Russian.\n6. Before you begin, take a deep breath. \n7. The report should not contain unnecessary information and should be clearly structured according to a template.  \n"),
            new Message(Role.User, message),
        };
        
        var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo_16K);
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;

        Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");

        return choice.Message;
    }
}