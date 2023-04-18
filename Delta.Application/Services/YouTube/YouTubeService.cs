using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Delta.Application.Services.YouTube;

public class YouTubeService : IYouTubeService
{
    private readonly HttpClient _httpClient;
    private readonly YouTubeOptions _options;
    
    public YouTubeService(IOptions<YouTubeOptions> options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Address);
    }


    public async Task<Video[]> Search(string query)
    {
        var response = await _httpClient.PostAsJsonAsync("/youtube/", new Request
        {
            Query = query
        });

        var answer = await response.Content.ReadFromJsonAsync<Answer>();
        
        return answer!.Response;
    }

    private class Answer
    {
        public Video[] Response { get; set; }
    }

    private class Request
    {
        public string Query { get; set; }
    }
}