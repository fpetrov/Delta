using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;

namespace Delta.Application.Services.Olimpiad;

public interface IOlimpiadService
{
    public Task Create(
        CreateOlimpiadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Все олимпиады, которые скоро пройдут.
    /// </summary>
    IAsyncEnumerable<Entities.Olimpiad> FindIncoming(
        OlimpiadType type,
        CancellationToken cancellationToken = default);
}