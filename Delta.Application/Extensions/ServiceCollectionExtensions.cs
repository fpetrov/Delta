using Microsoft.Extensions.DependencyInjection;
using Showcast.Core.Services.Http;
using Showcast.Infrastructure.Services.Http;

namespace Showcast.TelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMovieService(this IServiceCollection collection)
    {
        collection.AddHttpClient<IRecommendationService, RecommendationService>();
        collection.AddHttpClient<IMovieDbService, MovieDbService>();
        collection.AddScoped<IMovieService, MovieService>();
        
        
        return collection;
    }
}