using Delta.Core.Entities.Media;

namespace Delta.Core.Services.Http;

public interface IMovieDbService
{
    public Task<Movie?> GetById(string id);
    public Task<Movie?> GetByTitle(string title);
}