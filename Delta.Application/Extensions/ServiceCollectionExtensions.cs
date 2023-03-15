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
}