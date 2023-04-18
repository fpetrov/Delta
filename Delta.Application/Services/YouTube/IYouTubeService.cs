namespace Delta.Application.Services.YouTube;

public interface IYouTubeService
{
    public Task<Video[]> Search(string question);
}