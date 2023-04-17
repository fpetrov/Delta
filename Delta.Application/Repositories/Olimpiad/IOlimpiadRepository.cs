using Delta.Application.Entities;

namespace Delta.Application.Repositories.Olimpiad;

public interface IOlimpiadRepository
{
    public Task Create(
        Entities.Olimpiad olimpiad,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Entities.Olimpiad>> FindAllByType(
        OlimpiadType type,
        CancellationToken cancellationToken = default);
}