using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Dnevnik.Models;

namespace Delta.Application.Services.User;

public interface IUserService
{
    public Task Create(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    public Task<Schedule?> GetSchedule(
        GetScheduleRequest request,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Entities.User> FindAllByType(
        OlimpiadType olimpiadType,
        CancellationToken cancellationToken = default);

    public Task AddNotification(
        AddNotificationRequest request,
        CancellationToken cancellationToken = default);
}