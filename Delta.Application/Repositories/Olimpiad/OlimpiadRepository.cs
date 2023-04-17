using Delta.Application.Contexts;
using Delta.Application.Entities;

namespace Delta.Application.Repositories.Olimpiad;

public class OlimpiadRepository : RepositoryBase<Entities.Olimpiad, DatabaseContext>, IOlimpiadRepository
{
    public OlimpiadRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
        
    }

    public Task Create(Entities.Olimpiad olimpiad, CancellationToken cancellationToken = default)
    {
        return AddAsync(olimpiad, cancellationToken: cancellationToken);
    }
    
    public async Task Delete(int id, CancellationToken cancellationToken = default)
    {
        var olimpiad = await GetAsync(id, cancellationToken);
        
        await RemoveAsync(olimpiad, cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<Entities.Olimpiad>> FindAllByType(OlimpiadType type, CancellationToken cancellationToken = default)
    {
        return FindAllAsync(olimpiad => olimpiad.Type == type, cancellationToken);
    }
}