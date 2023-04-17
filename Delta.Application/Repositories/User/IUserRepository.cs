using Delta.Application.Entities;

namespace Delta.Application.Repositories.User;

public interface IUserRepository
{
    public Task Create(
        Entities.User user,
        CancellationToken cancellationToken = default);

    Task<Entities.User?> FindByTelegramId(
        long telegramId,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Entities.User> FindAllByType(
        OlimpiadType olimpiadType,
        CancellationToken cancellationToken = default);

    public Task Update(
        Entities.User user,
        CancellationToken cancellationToken = default);
}