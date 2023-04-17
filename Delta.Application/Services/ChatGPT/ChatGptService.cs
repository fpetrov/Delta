using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Delta.Application.Services.ChatGPT;

public class ChatGptService : IChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly ChatGptOptions _options;
    
    public ChatGptService(IOptions<ChatGptOptions> options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Address);
    }


    public async Task<string> Ask(string question)
    {
        var response = await _httpClient.PostAsJsonAsync<Request>("/ask/", new Request
        {
            Prompt = question
        });

        var answer = await response.Content.ReadFromJsonAsync<Answer>();
        
        return answer!.Response;
    }

    private class Answer
    {
        public string Response { get; set; }
    }

    private class Request
    {
        public string Prompt { get; set; }
    }
}