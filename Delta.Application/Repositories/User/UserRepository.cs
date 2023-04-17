using Delta.Application.Contexts;
using Delta.Application.Entities;

namespace Delta.Application.Repositories.User;

public class UserRepository : RepositoryBase<Entities.User, DatabaseContext>, IUserRepository
{
    public UserRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
        
    }

    public Task Create(Entities.User user, CancellationToken cancellationToken = default)
    {
        return AddAsync(user, cancellationToken: cancellationToken);
    }

    public async Task<Entities.User?> FindByTelegramId(long telegramId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(user => user.TelegramId == telegramId, cancellationToken);
    }

    public async IAsyncEnumerable<Entities.User> FindAllByType(
        OlimpiadType olimpiadType,
        CancellationToken cancellationToken = default)
    {
        foreach (var user in await GetAllAsync(cancellationToken))
        {
            if (user.OlimpiadType == olimpiadType)
                yield return user;
        }
    }
    
    public async IAsyncEnumerable<Entities.User> FindAll(
        CancellationToken cancellationToken = default)
    {
        foreach (var user in await GetAllAsync(cancellationToken))
        {
            yield return user;
        }
    }

    public Task Update(Entities.User user, CancellationToken cancellationToken = default)
    {
        return UpdateAsync(user, cancellationToken);
    }

    public Task DeleteNotification(
        Entities.User user,
        Notification notification,
        CancellationToken cancellationToken = default)
    {
        user.Notifications.Remove(notification);

        return Update(user, cancellationToken);
    }
}