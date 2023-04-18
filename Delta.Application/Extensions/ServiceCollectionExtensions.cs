using Delta.Application.Repositories.Olimpiad;
using Delta.Application.Repositories.User;
using Delta.Application.Services.ChatGPT;
using Delta.Application.Services.Olimpiad;
using Delta.Application.Services.User;
using Delta.Application.Services.YouTube;
using Delta.Dnevnik;
using Microsoft.Extensions.DependencyInjection;

namespace Delta.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserService(this IServiceCollection collection)
    {
        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IUserService, UserService>();

        return collection;
    }
    
    public static IServiceCollection AddOlimpiadService(this IServiceCollection collection)
    {
        collection.AddScoped<IOlimpiadRepository, OlimpiadRepository>();
        collection.AddScoped<IOlimpiadService, OlimpiadService>();

        return collection;
    }
    
    public static IServiceCollection AddChatGptService(this IServiceCollection collection, Action<ChatGptOptions> options)
    {
        collection.Configure(options);
        collection.AddHttpClient<IChatGptService, ChatGptService>();

        return collection;
    }
    
    public static IServiceCollection AddYouTubeService(this IServiceCollection collection, Action<YouTubeOptions> options)
    {
        collection.Configure(options);
        collection.AddHttpClient<IYouTubeService, YouTubeService>();

        return collection;
    }
}