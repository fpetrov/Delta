using Delta.Core.Messaging.Responses.Recommendation;

namespace Delta.Core.Services.Http;

public interface IRecommendationService
{
    public Task<string[]?> GetRecommendations(string likedMoviesTitles);

    public Task<RelativeMoviesResponse[]?> GetRelativeMovies(string movieName);
}
