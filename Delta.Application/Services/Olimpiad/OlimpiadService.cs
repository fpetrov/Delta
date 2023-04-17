using AutoMapper;
using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Application.Repositories.Olimpiad;

namespace Delta.Application.Services.Olimpiad;

public class OlimpiadService : IOlimpiadService
{
    private readonly IOlimpiadRepository _repository;
    private readonly IMapper _mapper;

    public OlimpiadService(IOlimpiadRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task Create(CreateOlimpiadRequest request, CancellationToken cancellationToken = default)
    {
        var olimpiad = _mapper.Map<Entities.Olimpiad>(request);

        return _repository.Create(olimpiad, cancellationToken);
    }

    public async IAsyncEnumerable<Entities.Olimpiad> FindIncoming(OlimpiadType type, CancellationToken cancellationToken = default)
    {
        var olimpiads = await _repository.FindAllByType(type, cancellationToken);

        foreach (var olimpiad in olimpiads)
        {
            var subtraction = olimpiad.Expires.Subtract(DateTime.Now);

            if (subtraction <= TimeSpan.FromDays(1))
            {
                yield return olimpiad;
            }
        }
    }
}