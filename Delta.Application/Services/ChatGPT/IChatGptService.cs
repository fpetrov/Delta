namespace Delta.Application.Services.ChatGPT;

public interface IChatGptService
{
    public Task<string> Ask(string question);
}