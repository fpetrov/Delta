using Delta.Core.Entities.Authentication;
using Delta.Core.Entities.Media;

namespace Delta.Core.Services.Http;

public interface IMovieService
{
    public IAsyncEnumerable<Movie?> GetRelative(int id);
    public IAsyncEnumerable<Movie?> GetRelative(string name);
    public Task<Movie?> GetMovie(string name);

    public IAsyncEnumerable<Movie?> GetRecommendations(User user);
}