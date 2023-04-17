using Delta.Application.Repositories.Olimpiad;
using Delta.Application.Repositories.User;
using Delta.Application.Services.Olimpiad;
using Delta.Application.Services.User;
using Delta.Dnevnik;
using Microsoft.Extensions.DependencyInjection;

namespace Delta.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMovieService(this IServiceCollection collection)
    {
        // collection.AddHttpClient<IRecommendationService, RecommendationService>();
        // collection.AddHttpClient<IMovieDbService, MovieDbService>();
        // collection.AddScoped<IMovieService, MovieService>();

        collection.AddHttpClient<IDnevnikConnection, DnevnikConnection>();
        
        return collection;
    }

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
}